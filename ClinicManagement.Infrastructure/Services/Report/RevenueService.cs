using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Report;
using ClinicManagement.Application.Interfaces.Report;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // EPPlus
using OfficeOpenXml.Style;

namespace ClinicManagement.Infrastructure.Services.Report
{
    public class RevenueService : IRevenueService
    {
        private readonly ClinicDbContext _ctx;

        public RevenueService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        // == Helpers ==
        private static IQueryable<Domain.Entity.Appointment> BaseAppointmentQuery(ClinicDbContext ctx)
        {
            return ctx.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Exam)
                .Where(a => a.Status != AppointmentStatus.Cancelled); // loại hủy
        }

        private static IQueryable<Domain.Entity.Appointment> ApplyTimeFilter(
            IQueryable<Domain.Entity.Appointment> q, DateTime? from, DateTime? to)
        {
            if (from.HasValue) q = q.Where(a => a.StartTime >= from.Value);
            if (to.HasValue) q = q.Where(a => a.StartTime <= to.Value);
            return q;
        }

        private static IQueryable<Domain.Entity.Appointment> ApplyKeyword(
            IQueryable<Domain.Entity.Appointment> q, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return q;
            var kw = keyword.Trim().ToLower();
            return q.Where(a =>
                a.Patient.FullName.ToLower().Contains(kw) ||
                a.Patient.Email.ToLower().Contains(kw) ||
                a.Doctor.FullName.ToLower().Contains(kw) ||
                (a.Exam != null && a.Exam.Name.ToLower().Contains(kw)) ||
                (!string.IsNullOrEmpty(a.TransactionCode) && a.TransactionCode!.ToLower().Contains(kw)));
        }

        // 1) Tổng đã trả / chưa trả
        public async Task<ServiceResult<PaymentOverviewDto>> GetPaymentOverviewAsync(DateTime? from = null, DateTime? to = null)
        {
            var q = ApplyTimeFilter(BaseAppointmentQuery(_ctx), from, to);

            // Tối ưu: chỉ lấy trường cần
            var data = await q.Select(a => new { a.IsPaid, Amount = a.TotalFee ?? 0m })
                              .ToListAsync();

            var paid = data.Where(x => x.IsPaid);
            var unpaid = data.Where(x => !x.IsPaid);

            var dto = new PaymentOverviewDto
            {
                TotalPaid = paid.Sum(x => x.Amount),
                CountPaid = paid.Count(),
                TotalUnpaid = unpaid.Sum(x => x.Amount),
                CountUnpaid = unpaid.Count()
            };

            return ServiceResult<PaymentOverviewDto>.Ok(dto);
        }

        // 2) Danh sách bệnh nhân (đã/ chưa trả) + lọc
        public async Task<ServiceResult<List<PaymentRecordDto>>> GetPaymentListAsync(
            bool? isPaid = null, DateTime? from = null, DateTime? to = null, string? keyword = null)
        {
            var q = BaseAppointmentQuery(_ctx);
            q = ApplyTimeFilter(q, from, to);
            q = ApplyKeyword(q, keyword);
            if (isPaid.HasValue) q = q.Where(a => a.IsPaid == isPaid.Value);

            var list = await q
                .OrderByDescending(a => a.StartTime)
                .Select(a => new PaymentRecordDto
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.Patient.FullName,
                    PatientEmail = a.Patient.Email,
                    DoctorName = a.Doctor.FullName,
                    ExamName = a.Exam != null ? a.Exam.Name : null,
                    StartTime = a.StartTime,
                    Amount = a.TotalFee ?? 0m,
                    IsPaid = a.IsPaid,
                    PaymentMethod = a.PaymentMethod,
                    TransactionCode = a.TransactionCode
                })
                .ToListAsync();

            return ServiceResult<List<PaymentRecordDto>>.Ok(list);
        }

        // 3) Doanh thu theo tháng trong năm (IsPaid=true)
        public async Task<ServiceResult<List<MonthlyRevenuePointDto>>> GetRevenueByMonthAsync(int? year = null)
        {
            var y = year ?? DateTime.UtcNow.Year;

            // Lấy dữ liệu thô (EF friendly)
            var raw = await _ctx.Appointments
                .Where(a => a.IsPaid && a.StartTime.Year == y && a.Status != AppointmentStatus.Cancelled)
                .Select(a => new { a.StartTime, Amount = a.TotalFee ?? 0m })
                .ToListAsync();

            var monthly = raw
                .GroupBy(x => x.StartTime.Month)
                .Select(g => new MonthlyRevenuePointDto
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.Amount),
                    AppointmentCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            return ServiceResult<List<MonthlyRevenuePointDto>>.Ok(monthly);
        }

        // 4) Doanh thu theo năm (IsPaid=true) trong khoảng [fromYear..toYear]
        public async Task<ServiceResult<List<YearlyRevenuePointDto>>> GetRevenueByYearRangeAsync(int fromYear, int toYear)
        {
            if (toYear < fromYear) (fromYear, toYear) = (toYear, fromYear);

            var raw = await _ctx.Appointments
                .Where(a => a.IsPaid && a.Status != AppointmentStatus.Cancelled &&
                            a.StartTime.Year >= fromYear && a.StartTime.Year <= toYear)
                .Select(a => new { Year = a.StartTime.Year, Amount = a.TotalFee ?? 0m })
                .ToListAsync();

            var yearly = raw
                .GroupBy(x => x.Year)
                .Select(g => new YearlyRevenuePointDto
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.Amount),
                    AppointmentCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ToList();

            return ServiceResult<List<YearlyRevenuePointDto>>.Ok(yearly);
        }

        // 5) Xuất Excel danh sách thanh toán (lọc giống list)
        public async Task<ServiceResult<byte[]>> ExportPaymentsToExcelAsync(
            bool? isPaid = null, DateTime? from = null, DateTime? to = null, string? keyword = null)
        {
            // EPPlus license context — tránh LicenseNotSetException
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var listResult = await GetPaymentListAsync(isPaid, from, to, keyword);
            var rows = listResult.Data ?? new List<PaymentRecordDto>();

            using var pkg = new ExcelPackage();
            var ws = pkg.Workbook.Worksheets.Add("Payments");

            // Header
            ws.Cells[1, 1].Value = "Appointment ID";
            ws.Cells[1, 2].Value = "Patient";
            ws.Cells[1, 3].Value = "Email";
            ws.Cells[1, 4].Value = "Doctor";
            ws.Cells[1, 5].Value = "Exam";
            ws.Cells[1, 6].Value = "Start Time";
            ws.Cells[1, 7].Value = "Amount (VND)";
            ws.Cells[1, 8].Value = "Paid?";
            ws.Cells[1, 9].Value = "Method";
            ws.Cells[1, 10].Value = "Txn Code";

            using (var r = ws.Cells[1, 1, 1, 10])
            {
                r.Style.Font.Bold = true;
                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AliceBlue);
            }

            int row = 2;
            foreach (var i in rows)
            {
                ws.Cells[row, 1].Value = i.AppointmentId;
                ws.Cells[row, 2].Value = i.PatientName;
                ws.Cells[row, 3].Value = i.PatientEmail;
                ws.Cells[row, 4].Value = i.DoctorName;
                ws.Cells[row, 5].Value = i.ExamName;
                ws.Cells[row, 6].Value = i.StartTime.ToString("dd/MM/yyyy HH:mm");
                ws.Cells[row, 7].Value = i.Amount;
                ws.Cells[row, 8].Value = i.IsPaid ? "Yes" : "No";
                ws.Cells[row, 9].Value = i.PaymentMethod;
                ws.Cells[row, 10].Value = i.TransactionCode;
                row++;
            }

            ws.Cells.AutoFitColumns();
            return ServiceResult<byte[]>.Ok(pkg.GetAsByteArray());
        }
    }
}
