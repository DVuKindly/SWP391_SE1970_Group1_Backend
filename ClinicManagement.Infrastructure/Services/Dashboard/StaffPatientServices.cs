using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Request.Booking;
using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Response.Auth;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Dashboard
{
    public class StaffPatientService : IStaffPatientService
    {
        private readonly ClinicDbContext _context;

        public StaffPatientService(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<List<RegistrationRequestResponseDto>>> GetAllRequestsAsync(string? status = null)
        {
            var query = _context.RegistrationRequests
                .Include(r => r.HandledBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            var list = await query
                .OrderByDescending(r => r.CreatedAtUtc)
                .Select(r => new RegistrationRequestResponseDto
                {
                    RegistrationRequestId = r.RegistrationRequestId,
                    FullName = r.FullName,
                    Email = r.Email,
                    Phone = r.Phone,
                    Content = r.Content,
                    StartDate = r.StartDate,
                    Status = r.Status,
                    PaymentStatus = r.PaymentStatus.ToString(), // 🧾 Thêm trạng thái thanh toán
                    IsProcessed = r.IsProcessed,
                    CreatedAtUtc = r.CreatedAtUtc,
                    HandledBy = r.HandledBy != null ? r.HandledBy.FullName : null,
                    ProcessedAt = r.ProcessedAt,
                    InternalNote = r.InternalNote
                })
                .ToListAsync();

            return ServiceResult<List<RegistrationRequestResponseDto>>.Ok(list);
        }



        // 🔹 Lấy chi tiết 1 đăng ký khám
        public async Task<ServiceResult<RegistrationRequestDetailDto>> GetRequestDetailAsync(int requestId)
        {
            var req = await _context.RegistrationRequests
                .Include(r => r.HandledBy)
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(r => r.RegistrationRequestId == requestId);

            if (req == null)
                return ServiceResult<RegistrationRequestDetailDto>.Fail("Không tìm thấy đăng ký khám.");

            var dto = new RegistrationRequestDetailDto
            {
                RegistrationRequestId = req.RegistrationRequestId,
                FullName = req.FullName,
                Email = req.Email,
                Phone = req.Phone,
                Content = req.Content,
                StartDate = req.StartDate,
                Status = req.Status,
                PaymentStatus = req.PaymentStatus.ToString(), // 🧾 Thêm
                IsProcessed = req.IsProcessed,
                InternalNote = req.InternalNote,
                HandledBy = req.HandledBy?.FullName,
                ProcessedAt = req.ProcessedAt,
                AppointmentInfo = req.Appointment != null
                    ? $"Lịch {req.Appointment.StartTime:HH:mm dd/MM/yyyy} với BS {req.Appointment.Doctor?.FullName}"
                    : null,
               
            };

            return ServiceResult<RegistrationRequestDetailDto>.Ok(dto);
        }


        // 🔹 Cập nhật trạng thái đăng ký
        public async Task<ServiceResult<string>> UpdateStatusAsync(int requestId, string newStatus, int staffId)
        {
            var req = await _context.RegistrationRequests.FindAsync(requestId);
            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

            // ❌ Không cho cập nhật nếu đã thanh toán
            if (req.Status == "Paid" || req.Status == "Direct_Payment")
                return ServiceResult<string>.Fail("Không thể cập nhật trạng thái vì đăng ký đã được thanh toán.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            // ⚠️ Validate không cho revert trạng thái
            if ((req.Status == "Scheduled" || req.Status == "Examined") &&
                (newStatus == "Contacted" || newStatus == "Invalid"))
            {
                return ServiceResult<string>.Fail(
                    $"Không thể chuyển từ trạng thái '{req.Status}' về '{newStatus}' vì đã lên lịch hoặc đã khám."
                );
            }

            // ⚠️ Validate nếu trạng thái yêu cầu không hợp lệ trong danh sách
            if (!_validStatuses.Contains(newStatus))
                return ServiceResult<string>.Fail("Trạng thái mới không hợp lệ.");

          
            string oldStatus = req.Status;
            req.Status = newStatus;
            req.HandledById = staffId;
            req.IsProcessed = true;
            req.ProcessedAt = DateTime.UtcNow;
            req.UpdatedAtUtc = DateTime.UtcNow;

      
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix +
                $"Cập nhật trạng thái từ '{oldStatus}' → '{newStatus}'.";

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Cập nhật trạng thái thành công: {oldStatus} → {newStatus}");
        }

        // 🔹 Thêm ghi chú nội bộ
        public async Task<ServiceResult<string>> AddNoteAsync(int requestId, int staffId, string note)
        {
            var req = await _context.RegistrationRequests.FindAsync(requestId);
            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            // Append ghi chú
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix + note;
            req.HandledById = staffId;
            req.ProcessedAt = DateTime.UtcNow;

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok("Đã thêm ghi chú vào đăng ký khám.");
        }


        // 🔹 Đánh dấu đăng ký khám là không hợp lệ / ảo
        public async Task<ServiceResult<string>> MarkAsInvalidAsync(int requestId, int staffId, string reason)
        {
            var req = await _context.RegistrationRequests
                .Include(r => r.Appointment)
                .FirstOrDefaultAsync(r => r.RegistrationRequestId == requestId);

            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

            // ❌ Không cho đánh dấu không hợp lệ nếu đã thanh toán
            if (req.Status == "Paid" || req.Status == "Direct_Payment")
                return ServiceResult<string>.Fail("Không thể đánh dấu không hợp lệ vì đăng ký đã được thanh toán.");

            // ❌ Không cho đánh dấu không hợp lệ nếu đã lên lịch hoặc đã khám
            if (req.Status == "Scheduled" || req.Status == "Examined")
                return ServiceResult<string>.Fail("Không thể đánh dấu không hợp lệ vì đăng ký đã lên lịch hoặc đã được khám.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

           
            string oldStatus = req.Status;
            req.Status = "Invalid";
            req.IsProcessed = true;
            req.HandledById = staffId;
            req.ProcessedAt = DateTime.UtcNow;
            req.UpdatedAtUtc = DateTime.UtcNow;

       
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix +
                $"Đánh dấu đăng ký là 'Invalid' (không hợp lệ). Lý do: {reason}. Trạng thái cũ: {oldStatus}.";

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Đã đánh dấu đăng ký #{requestId} là 'Invalid' thành công.");
        }


        // 🔹 Xác nhận thanh toán trực tiếp (Direct_Payment)

        public async Task<ServiceResult<string>> ExecuteDirectPaymentAsync(int requestId, int staffId, int examId)
        {
            var req = await _context.RegistrationRequests
                .Include(r => r.Exam)
                .FirstOrDefaultAsync(r => r.RegistrationRequestId == requestId);

            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

            if (req.Status == "Paid" || req.Status == "Direct_Payment")
                return ServiceResult<string>.Fail("Đăng ký này đã được thanh toán, không thể thực hiện lại.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.ExamId == examId && e.IsActive);
            if (exam == null)
                return ServiceResult<string>.Fail("Không tìm thấy gói khám hợp lệ.");

            req.ExamId = exam.ExamId;
            req.Exam = exam;
            req.Fee = exam.Price;
            req.Status = "Direct_Payment";
            req.IsProcessed = true;
            req.HandledById = staffId;
            req.ProcessedAt = DateTime.UtcNow;
            req.UpdatedAtUtc = DateTime.UtcNow;

            // Ghi log
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix +
                $"Xác nhận thanh toán trực tiếp tại phòng khám. Gói khám: {exam.Name} ({exam.Price:N0} VNĐ)";

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Đã xác nhận thanh toán trực tiếp cho đăng ký #{requestId} với gói khám '{exam.Name}'.");
        }


        // lọc 
        private static readonly HashSet<string> _validStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "New", "Contacted", "Invalid", "Direct_Payment", "Paid", "Pending", "Rejected" , "Scheduled"
        };
        public async Task<ServiceResult<PagedResult<RegistrationRequestResponseDto>>> GetRequestsAsync(
           string? status = null,
           string? email = null,
           int page = 1,
           int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = _context.RegistrationRequests
                .AsNoTracking()
                .Include(r => r.HandledBy)
                .OrderByDescending(r => r.CreatedAtUtc)
                .AsQueryable();

            // Lọc trạng thái
            if (!string.IsNullOrWhiteSpace(status) && _validStatuses.Contains(status))
            {
                query = query.Where(r => r.Status == status);
            }

            // Tìm kiếm email
            if (!string.IsNullOrWhiteSpace(email))
            {
                var keyword = email.Trim().ToLower();
                query = query.Where(r => r.Email.ToLower().Contains(keyword));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RegistrationRequestResponseDto
                {
                    RegistrationRequestId = r.RegistrationRequestId,
                    FullName = r.FullName,
                    Email = r.Email,
                    Phone = r.Phone,
                    Content = r.Content,
                    StartDate = r.StartDate,
                    Status = r.Status,
                    IsProcessed = r.IsProcessed,
                    CreatedAtUtc = r.CreatedAtUtc,
                    HandledBy = r.HandledBy != null ? r.HandledBy.FullName : null,
                    ProcessedAt = r.ProcessedAt,
                    InternalNote = r.InternalNote
                })
                .ToListAsync();

            var pagedResult = new PagedResult<RegistrationRequestResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = items
            };

            return ServiceResult<PagedResult<RegistrationRequestResponseDto>>.Ok(pagedResult);
        }

        // 🔹 Đánh dấu đăng ký đã khám
        public async Task<ServiceResult<string>> MarkAsExaminedAsync(int requestId, int staffId)
        {
            var req = await _context.RegistrationRequests
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Exam)
                .Include(r => r.Exam)
                .FirstOrDefaultAsync(r => r.RegistrationRequestId == requestId);

            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký khám.");

            var appointment = req.Appointment;

            // ❌ Validate trạng thái không hợp lệ
            if (req.Status == "Invalid" || req.Status == "Rejected")
                return ServiceResult<string>.Fail("Không thể đánh dấu 'Đã khám' cho đăng ký không hợp lệ hoặc bị từ chối.");

            if (req.Status == "Examined")
                return ServiceResult<string>.Fail("Đăng ký này đã được đánh dấu là 'Đã khám' trước đó.");

            // 🔹 Trường hợp đã thanh toán online (VNPay)
            if (req.Status == "Paid")
            {
                // Đảm bảo có lịch hẹn và đã thanh toán
                if (appointment != null && appointment.IsPaid)
                    return await MarkExaminedCore(req, staffId, "Đã thanh toán qua VNPay.");
                else
                    return ServiceResult<string>.Fail("Thanh toán qua VNPay chưa được ghi nhận đầy đủ. Vui lòng kiểm tra lại.");
            }

            // 🔹 Trường hợp thanh toán trực tiếp tại quầy
            if (req.Status == "Direct_Payment")
            {
                if (appointment == null)
                    return ServiceResult<string>.Fail("Không tìm thấy lịch hẹn để xác nhận thanh toán.");

                if (appointment.IsPaid)
                    return await MarkExaminedCore(req, staffId, "Đã thanh toán trực tiếp tại quầy (có phiếu thu).");
                else
                    return ServiceResult<string>.Fail("Bệnh nhân thanh toán trực tiếp nhưng chưa có phiếu thu. Vui lòng tạo phiếu thu trước.");
            }

            // 🔹 Trường hợp đã xếp lịch (Scheduled)
            if (req.Status == "Scheduled")
            {
                if (appointment == null)
                    return ServiceResult<string>.Fail("Không tìm thấy lịch hẹn tương ứng.");

                // Nếu lịch hẹn đã thanh toán (qua quầy hoặc VNPay)
                if (appointment.IsPaid)
                    return await MarkExaminedCore(req, staffId, "Đã lên lịch và xác nhận thanh toán thành công.");
                else
                    return ServiceResult<string>.Fail("Bệnh nhân chưa thanh toán. Không thể đánh dấu đã khám.");
            }

            // 🔹 Các trạng thái còn lại
            return ServiceResult<string>.Fail($"Không thể đánh dấu đã khám khi đăng ký đang ở trạng thái '{req.Status}'.");
        }


        // ✅ Helper nội bộ để tránh lặp logic
        private async Task<ServiceResult<string>> MarkExaminedCore(RegistrationRequest req, int staffId, string reason)
        {
            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            req.Status = "Examined";
            req.IsProcessed = true;
            req.HandledById = staffId;
            req.ProcessedAt = DateTime.UtcNow;
            req.UpdatedAtUtc = DateTime.UtcNow;

            // 🔹 Cập nhật lịch hẹn: Completed nếu có
            if (req.Appointment != null)
            {
                req.Appointment.Status = AppointmentStatus.Completed;
                req.Appointment.UpdatedAtUtc = DateTime.UtcNow;
                _context.Appointments.Update(req.Appointment);
            }

            // 🔹 Ghi log
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix +
                $"Đánh dấu 'Đã khám'. {reason} Gói khám: {req.Exam?.Name ?? "N/A"}.";

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Đã cập nhật đăng ký #{req.RegistrationRequestId} thành 'Đã khám' thành công.");
        }





    }
}
