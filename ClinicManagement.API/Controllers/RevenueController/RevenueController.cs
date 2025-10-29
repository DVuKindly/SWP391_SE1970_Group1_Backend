using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Report;
using ClinicManagement.Application.Interfaces.Report;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Report
{
    [ApiController]
    [Route("api/revenue")]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _svc;

        public RevenueController(IRevenueService svc)
        {
            _svc = svc;
        }

        [HttpGet("patient-list")]
        public async Task<IActionResult> GetPatientList(
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null,
    [FromQuery] string? keyword = null)
        {
            // Lấy toàn bộ danh sách (không filter isPaid)
            var result = await _svc.GetPaymentListAsync(null, from, to, keyword);
            if (!result.Success) return BadRequest(result);

            // Gom thêm thông tin tổng quan cho FE hiển thị ngay (nếu cần)
            var totalPaid = result.Data!.Where(x => x.IsPaid).Sum(x => x.Amount);
            var totalUnpaid = result.Data!.Where(x => !x.IsPaid).Sum(x => x.Amount);
            var totalCount = result.Data!.Count;

            var response = new
            {
                TotalPatients = totalCount,
                PaidTotal = totalPaid,
                UnpaidTotal = totalUnpaid,
                Patients = result.Data!
            };

            return Ok(ServiceResult<object>.Ok(response, "Danh sách bệnh nhân và thanh toán"));
        }

        /// View tổng tiền đã trả / chưa trả (lọc theo from/to)
        [HttpGet("payment-overview")]
        public async Task<IActionResult> GetPaymentOverview([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var rs = await _svc.GetPaymentOverviewAsync(from, to);
            return rs.Success ? Ok(rs) : BadRequest(rs);
        }

        /// Danh sách thanh toán: filter isPaid / from-to / keyword
        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments(
            [FromQuery] bool? isPaid = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] string? keyword = null)
        {
            var rs = await _svc.GetPaymentListAsync(isPaid, from, to, keyword);
            return rs.Success ? Ok(rs) : BadRequest(rs);
        }

        /// Doanh thu theo tháng trong năm (IsPaid = true)
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth([FromQuery] int? year = null)
        {
            var rs = await _svc.GetRevenueByMonthAsync(year);
            return rs.Success ? Ok(rs) : BadRequest(rs);
        }

        /// Doanh thu theo năm (IsPaid = true) trong khoảng
        [HttpGet("by-year")]
        public async Task<IActionResult> GetByYear([FromQuery] int fromYear, [FromQuery] int toYear)
        {
            var rs = await _svc.GetRevenueByYearRangeAsync(fromYear, toYear);
            return rs.Success ? Ok(rs) : BadRequest(rs);
        }

        /// Xuất Excel theo bộ lọc danh sách thanh toán
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel(
            [FromQuery] bool? isPaid = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] string? keyword = null)
        {
            var rs = await _svc.ExportPaymentsToExcelAsync(isPaid, from, to, keyword);
            if (!rs.Success || rs.Data == null) return BadRequest(rs);
            var fileName = $"Payments_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(rs.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
