using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Application.Interfaces.Services.Registration;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ClinicDbContext _ctx;
        private readonly IEmailService _email;

        public RegistrationService(ClinicDbContext ctx, IEmailService email)
        {
            _ctx = ctx;
            _email = email;
        }

        public async Task<ServiceResult<int>> CreateRegistrationAsync(
            RegistrationRequestDto req,
            CancellationToken ct = default)
        {
            // 1. Tạo RegistrationRequest
            var reg = new RegistrationRequest
            {
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim().ToLower(),
                Phone = req.Phone.Trim(),
                Content = req.Content.Trim(),
                IsProcessed = false,
                StartDate = req.StartDate,
                CreatedAtUtc = DateTime.UtcNow
            };

            _ctx.RegistrationRequests.Add(reg);

            // 2. Nếu chưa có Patient thì tạo account
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

                // 3. Gửi email cho bệnh nhân
                var subject = "Thông tin đăng ký khám tại Clinic";
                var body = $@"
                    <p>Xin chào {reg.FullName},</p>
                    <p>Bạn đã đăng ký khám thành công. Đây là thông tin tài khoản:</p>
                    <ul>
                        <li>Email: {reg.Email}</li>
                        <li>Password: <b>{randomPwd}</b></li>
                    </ul>
                    <p>Vui lòng đăng nhập và đổi mật khẩu sau khi đăng nhập lần đầu.</p>
                    <p>Trân trọng,<br/>Clinic Team</p>
                ";

                await _email.SendEmailAsync(reg.Email, subject, body);
            }

            // 4. Lưu thay đổi DB
            await _ctx.SaveChangesAsync(ct);

            return ServiceResult<int>.Ok(
                reg.RegistrationRequestId,
                "Đăng ký khám thành công, vui lòng kiểm tra email để nhận thông tin đăng nhập."
            );
        }
    }
}
