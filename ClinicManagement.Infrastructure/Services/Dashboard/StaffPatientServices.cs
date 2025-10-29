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
                IsProcessed = req.IsProcessed,
                InternalNote = req.InternalNote,
                HandledBy = req.HandledBy?.FullName,
                ProcessedAt = req.ProcessedAt,
                AppointmentInfo = req.Appointment != null
                    ? $"Lịch {req.Appointment.StartTime:HH:mm dd/MM/yyyy} với BS {req.Appointment.Doctor?.FullName}"
                    : null
            };

            return ServiceResult<RegistrationRequestDetailDto>.Ok(dto);
        }

        // 🔹 Cập nhật trạng thái đăng ký
        public async Task<ServiceResult<string>> UpdateStatusAsync(int requestId, string newStatus, int staffId)
        {
            var req = await _context.RegistrationRequests.FindAsync(requestId);
            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

         
            if (req.Status == "Paid" || req.Status == "Direct_Payment")
                return ServiceResult<string>.Fail("Không thể cập nhật trạng thái vì đăng ký đã được thanh toán.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            req.Status = newStatus;
            req.HandledById = staffId;
            req.IsProcessed = true;
            req.ProcessedAt = DateTime.UtcNow;

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Cập nhật trạng thái thành công: {newStatus}");
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
            var req = await _context.RegistrationRequests.FindAsync(requestId);
            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

           
            if (req.Status == "Paid" || req.Status == "Direct_Payment")
                return ServiceResult<string>.Fail("Không thể đánh dấu không hợp lệ vì đăng ký đã được thanh toán.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            req.Status = "Invalid"; // hoặc "Unknown"
            req.InternalNote = (req.InternalNote ?? "") +
                               $"\n[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: Đánh dấu không hợp lệ - {reason}";
            req.HandledById = staffId;
            req.IsProcessed = true;
            req.ProcessedAt = DateTime.UtcNow;

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Đã đánh dấu đăng ký #{requestId} là 'Invalid'.");
        }

        // 🔹 Xác nhận thanh toán trực tiếp (Direct_Payment)
        public async Task<ServiceResult<string>> ExecuteDirectPaymentAsync(int requestId, int staffId)
        {
            var req = await _context.RegistrationRequests.FindAsync(requestId);
            if (req == null)
                return ServiceResult<string>.Fail("Không tìm thấy đăng ký.");

            // ❌ Nếu đã thanh toán rồi thì không thực hiện lại
            if (req.Status == "Paid" || req.Status == "Direct_Payment")
                return ServiceResult<string>.Fail("Đăng ký này đã được thanh toán, không thể thực hiện lại.");

            var staff = await _context.Employees.FindAsync(staffId);
            if (staff == null)
                return ServiceResult<string>.Fail("Nhân viên không tồn tại.");

            req.Status = "Direct_Payment";
            req.IsProcessed = true;
            req.HandledById = staffId;
            req.ProcessedAt = DateTime.UtcNow;

            // Ghi log nội bộ
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] {staff.FullName}: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix + "Xác nhận thanh toán trực tiếp tại phòng khám.";

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.Ok($"Đã xác nhận thanh toán trực tiếp cho đăng ký #{requestId}.");
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


    }
}
