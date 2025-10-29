using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Appointment;
using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Response.Appoitment;

using ClinicManagement.Application.Interfaces.Appoiment;
using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Appoiment
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ClinicDbContext _context;
        private readonly IEmailService _email;
        private static DateTime NowVN => DateTime.UtcNow.AddHours(7);
        public AppointmentService(ClinicDbContext context , IEmailService email)
        {
            _context = context;
            _email = email;
        }

        #region  1. Hiển thị bệnh nhân đủ điều kiện đặt lịch
        public async Task<ServiceResult<List<EligiblePatientResponseDto>>> GetEligiblePatientsAsync()
        {
            var list = await _context.RegistrationRequests
                .Include(r => r.Exam)
                .Where(r => r.Status == "Paid" || r.Status == "Direct_Payment")
                .Select(r => new EligiblePatientResponseDto
                {
                    RegistrationRequestId = r.RegistrationRequestId,
                    FullName = r.FullName,
                    Email = r.Email,
                    Phone = r.Phone,
                    ExamName = r.Exam != null ? r.Exam.Name : null,
                    Fee = r.Fee
                })
                .OrderByDescending(r => r.RegistrationRequestId)
                .ToListAsync();

            return ServiceResult<List<EligiblePatientResponseDto>>.Ok(list);
        }
        #endregion

        #region 🩺 2. Hiển thị toàn bộ lịch hẹn
        public async Task<ServiceResult<List<AppointmentResponseDto>>> GetAllAppointmentsAsync(string? status = null)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Exam)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
                query = query.Where(a => a.Status == parsedStatus);

            var list = await query
                .OrderByDescending(a => a.StartTime)
                .Select(a => new AppointmentResponseDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorName = a.Doctor.FullName,
                    PatientName = a.Patient.FullName,
                    ExamName = a.Exam != null ? a.Exam.Name : "",
                    TotalFee = a.TotalFee,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status.ToString(),
                    Note = a.Note,
                    IsPaid = a.IsPaid,
                    PaymentMethod = a.PaymentMethod,
                    TransactionCode = a.TransactionCode
                })
                .ToListAsync();

            return ServiceResult<List<AppointmentResponseDto>>.Ok(list);
        }
        #endregion

            #region 🧠 3. Lấy danh sách bác sĩ + lịch làm việc
            public async Task<ServiceResult<List<DoctorScheduleResponseDto>>> GetAllDoctorsWithWorkPatternsAsync()
            {
                var doctors = await _context.Employees
                    .Include(e => e.DoctorProfile)
                    .Include(e => e.DoctorDepartments)
                        .ThenInclude(dd => dd.Department)
                    .Where(e => e.EmployeeRoles.Any(r => r.Role.Name == "Doctor") && e.IsActive)
                    .ToListAsync();

                var result = new List<DoctorScheduleResponseDto>();

                foreach (var doc in doctors)
                {
                    var patterns = await _context.DoctorWorkPatterns
                        .Where(w => w.DoctorId == doc.EmployeeUserId && w.IsWorking)
                        .OrderBy(w => w.DayOfWeek)
                        .Select(w => new WorkPatternDto
                        {
                            DayOfWeek = (int)w.DayOfWeek,
                            DayName = w.DayOfWeek.ToString(),
                            StartTime = w.StartTime.ToString(@"hh\:mm"),
                            EndTime = w.EndTime.ToString(@"hh\:mm"),
                            IsWorking = w.IsWorking
                        })
                        .ToListAsync();

                    result.Add(new DoctorScheduleResponseDto
                    {
                        DoctorId = doc.EmployeeUserId,
                        FullName = doc.FullName,
                        DepartmentName = doc.DoctorDepartments.FirstOrDefault()?.Department?.Name,
                        Title = doc.DoctorProfile?.Title,
                        Degree = doc.DoctorProfile?.Degree,
                        Image = doc.Image,
                        IsActive = doc.IsActive,
                        WorkPatterns = patterns
                    });
                }

                return ServiceResult<List<DoctorScheduleResponseDto>>.Ok(result);
            }
        #endregion

        #region 📅 4. Tạo lịch hẹn mới (dùng RegistrationRequestId)
        public async Task<ServiceResult<AppointmentResponseDto>> CreateAppointmentAsync(AppointmentRequestDto request, int createdById)
        {
            try
            {
                var regRequest = await _context.RegistrationRequests
                    .Include(r => r.Exam)
                    .FirstOrDefaultAsync(r => r.RegistrationRequestId == request.RegistrationRequestId);

                if (regRequest == null)
                    return ServiceResult<AppointmentResponseDto>.Fail("Không tìm thấy yêu cầu đăng ký khám.");

                if (regRequest.Status != "Paid" && regRequest.Status != "Direct_Payment")
                    return ServiceResult<AppointmentResponseDto>.Fail("Bệnh nhân chưa thanh toán hoặc không đủ điều kiện đặt lịch.");

                var exam = regRequest.Exam!;
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == regRequest.Email);
                if (patient == null)
                    return ServiceResult<AppointmentResponseDto>.Fail("Không tìm thấy bệnh nhân tương ứng.");

                // 🔹 Kiểm tra ca làm việc
                var workPatterns = await _context.DoctorWorkPatterns
                    .Where(w => w.DoctorId == request.DoctorId &&
                                w.DayOfWeek == request.StartTime.DayOfWeek &&
                                w.IsWorking)
                    .ToListAsync();

                if (!workPatterns.Any())
                {
                    workPatterns = await _context.WorkPatternTemplates
                        .Where(t => t.DayOfWeek == request.StartTime.DayOfWeek && t.IsActive)
                        .Select(t => new DoctorWorkPattern
                        {
                            StartTime = t.StartTime,
                            EndTime = t.EndTime,
                            SlotMinutes = t.SlotMinutes,
                            IsWorking = true
                        })
                        .ToListAsync();
                }

                bool inWorkingTime = workPatterns.Any(w =>
                    request.StartTime.TimeOfDay >= w.StartTime &&
                    request.EndTime.TimeOfDay <= w.EndTime);

                if (!inWorkingTime)
                    return ServiceResult<AppointmentResponseDto>.Fail("Bác sĩ không làm việc trong khung giờ này.");

                // 🔹 Kiểm tra trùng lịch
                bool conflict = await _context.Appointments.AnyAsync(a =>
                    a.DoctorId == request.DoctorId &&
                    a.StartTime < request.EndTime &&
                    a.EndTime > request.StartTime &&
                    a.Status != AppointmentStatus.Cancelled);

                if (conflict)
                    return ServiceResult<AppointmentResponseDto>.Fail("Khung giờ này đã có lịch hẹn khác.");

                // ✅ Xác định phương thức và trạng thái thanh toán
                string paymentMethod = regRequest.Status switch
                {
                    "Paid" => "VNPay",
                    "Direct_Payment" => "Direct_Payment",
                    _ => "Unknown"
                };

                bool isPaid = paymentMethod == "VNPay"; // 🔹 Chỉ VNPay mới coi là đã trả tiền ngay

                // 🔹 Tạo lịch hẹn
                var appointment = new Appointment
                {
                    PatientId = patient.PatientUserId,
                    DoctorId = request.DoctorId,
                    ExamId = regRequest.ExamId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Note = request.Note,
                    CreatedById = createdById,
                    Status = AppointmentStatus.Pending,
                    TotalFee = regRequest.Fee ?? exam.Price,
                    PaymentMethod = paymentMethod,
                    IsPaid = isPaid,
                    CreatedAtUtc = NowVN
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // 🔹 Cập nhật RegistrationRequest
                regRequest.AppointmentId = appointment.AppointmentId;
                regRequest.Status = "Scheduled";
                regRequest.UpdatedAtUtc = NowVN;
                await _context.SaveChangesAsync();

                var doctor = await _context.Employees.FindAsync(request.DoctorId);

                // ✅ Gửi email xác nhận cho bệnh nhân
                if (!string.IsNullOrWhiteSpace(regRequest.Email))
                {
                    var apptDate = appointment.StartTime.ToString("HH:mm dd/MM/yyyy");
                    string subject = $"[ClinicCare] Đã tiếp nhận lịch hẹn của bạn - {apptDate}";
                    string body = $@"
<div style='font-family:Arial; line-height:1.6'>
  <h2 style='color:#2A4D9B;'>Xác nhận đặt lịch hẹn</h2>
  <p>Xin chào <strong>{regRequest.FullName}</strong>,</p>
  <p>Chúng tôi đã tiếp nhận yêu cầu đặt lịch của bạn.</p>
  <ul>
    <li><strong>Dịch vụ:</strong> {exam.Name}</li>
    <li><strong>Bác sĩ:</strong> {doctor?.FullName}</li>
    <li><strong>Thời gian:</strong> {apptDate}</li>
    <li><strong>Phương thức thanh toán:</strong> {(paymentMethod == "VNPay" ? "Thanh toán online (VNPay)" : "Thanh toán tại quầy")}</li>
    <li><strong>Trạng thái thanh toán:</strong> {(isPaid ? "Đã thanh toán" : "Chưa thanh toán")}</li>
    <li><strong>Chi phí dự kiến:</strong> {(appointment.TotalFee ?? 0):N0} VNĐ</li>
    <li><strong>Trạng thái lịch hẹn:</strong> {appointment.Status}</li>
  </ul>
  <p>Chúng tôi sẽ sớm xác nhận hoặc điều chỉnh lịch nếu cần.</p>
  <hr style='border:none;border-top:1px solid #ccc;margin:20px 0'/>
  <p>Trân trọng,<br/>Đội ngũ <strong>ClinicCare</strong></p>
</div>";

                    await _email.SendEmailAsync(regRequest.Email, subject, body);
                }

                var response = new AppointmentResponseDto
                {
                    AppointmentId = appointment.AppointmentId,
                    DoctorName = doctor?.FullName ?? "",
                    PatientName = regRequest.FullName,
                    ExamName = exam.Name,
                    TotalFee = appointment.TotalFee,
                    StartTime = appointment.StartTime,
                    EndTime = appointment.EndTime,
                    Status = appointment.Status.ToString(),
                    Note = appointment.Note,
                    PaymentMethod = appointment.PaymentMethod,
                    IsPaid = appointment.IsPaid
                };

                return ServiceResult<AppointmentResponseDto>.Ok(response, "Đặt lịch thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult<AppointmentResponseDto>.Fail("Lỗi khi đặt lịch: " + ex.Message);
            }
        }
        #endregion


        #region  5. Chi tiết / Xoá / Duyệt
        public async Task<ServiceResult<AppointmentResponseDto>> GetAppointmentDetailAsync(int id)
        {
            var a = await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                .Include(x => x.Exam)
                .FirstOrDefaultAsync(x => x.AppointmentId == id);

            if (a == null) return ServiceResult<AppointmentResponseDto>.Fail("Không tìm thấy lịch hẹn.");

            var dto = new AppointmentResponseDto
            {
                AppointmentId = a.AppointmentId,
                DoctorName = a.Doctor.FullName,
                PatientName = a.Patient.FullName,
                ExamName = a.Exam?.Name ?? "",
                TotalFee = a.TotalFee,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status.ToString(),
                Note = a.Note,
                IsPaid = a.IsPaid,
                PaymentMethod = a.PaymentMethod,
                TransactionCode = a.TransactionCode
            };

            return ServiceResult<AppointmentResponseDto>.Ok(dto);
        }

        public async Task<ServiceResult<bool>> ApproveAppointmentAsync(int id, int approvedById, bool approve)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return ServiceResult<bool>.Fail("Không tìm thấy lịch hẹn.");

            appointment.Status = approve ? AppointmentStatus.Confirmed : AppointmentStatus.Cancelled;
            appointment.ApprovedById = approvedById;
            appointment.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, approve ? "Đã xác nhận lịch hẹn." : "Đã huỷ lịch hẹn.");
        }

        public async Task<ServiceResult<bool>> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return ServiceResult<bool>.Fail("Không tìm thấy lịch hẹn.");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Đã xoá lịch hẹn.");
        }

        // lọc
        public async Task<ServiceResult<List<AppointmentResponseDto>>> GetAllAppointmentsAsync(string? status = null, string? keyword = null)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Exam)
                .AsQueryable();

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
                query = query.Where(a => a.Status == parsedStatus);

            // Tìm kiếm theo email 
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(a =>
                    a.Patient.Email.ToLower().Contains(kw) ||
                    a.Patient.FullName.ToLower().Contains(kw) ||
                    a.Doctor.FullName.ToLower().Contains(kw));
            }

            
            var list = await query
                .OrderByDescending(a => a.StartTime)
                .Select(a => new AppointmentResponseDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorName = a.Doctor.FullName,
                    PatientName = a.Patient.FullName,
                    ExamName = a.Exam != null ? a.Exam.Name : "",
                    TotalFee = a.TotalFee,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status.ToString(),
                    Note = a.Note,
                    IsPaid = a.IsPaid,
                    PaymentMethod = a.PaymentMethod,
                    TransactionCode = a.TransactionCode
                })
                .ToListAsync();

            return ServiceResult<List<AppointmentResponseDto>>.Ok(list);
        }

        #endregion
    }
}
