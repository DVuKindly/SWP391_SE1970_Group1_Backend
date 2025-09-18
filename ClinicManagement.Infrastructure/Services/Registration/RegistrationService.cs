using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Auth;

using ClinicManagement.Application.Interfaces.Services.Registration;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ClinicDbContext _ctx;

        public RegistrationService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ServiceResult<int>> CreateRegistrationAsync(RegistrationRequestDto req, CancellationToken ct = default)
        {
           
            var reg = new RegistrationRequest
            {
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim().ToLower(),
                Phone = req.Phone.Trim(),
                Content = req.Content.Trim(),
                IsProcessed = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            _ctx.RegistrationRequests.Add(reg);

            // 2. Nếu chưa có Patient thì tạo account luôn
            var existing = await _ctx.Patients.FirstOrDefaultAsync(p => p.Email == reg.Email, ct);
            if (existing == null)
            {
                var randomPwd = Guid.NewGuid().ToString("N")[..8];
                var hash = BCrypt.Net.BCrypt.HashPassword(randomPwd);

                var patient = new Patient
                {
                    Email = reg.Email,
                    PasswordHash = hash,
                    FullName = reg.FullName,
                    Phone = reg.Phone,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _ctx.Patients.Add(patient);

                Console.WriteLine($"[DEBUG] Password cho {reg.Email}: {randomPwd}");
            }

            await _ctx.SaveChangesAsync(ct);

            return ServiceResult<int>.Ok(reg.RegistrationRequestId, "Đăng ký khám thành công, vui lòng chờ nhân viên liên hệ xác nhận.");
        }
    }
}
