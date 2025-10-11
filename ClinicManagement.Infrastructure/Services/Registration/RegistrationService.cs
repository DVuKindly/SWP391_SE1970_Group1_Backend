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
            
            var existRequest = await _ctx.RegistrationRequests
                .Where(r => r.Email == req.Email && r.Status == "Pending")
                .FirstOrDefaultAsync(ct);
            if (existRequest != null)
            {
                return ServiceResult<int>.Fail("Bạn đã gửi đăng ký và đang chờ xử lý.");
            }

      
            var reg = new RegistrationRequest
            {
                FullName = req.FullName.Trim(),
                Email = req.Email.Trim().ToLower(),
                Phone = req.Phone.Trim(),
                Content = req.Content.Trim(),
                StartDate = req.StartDate,
                Status = "Pending",       
                IsProcessed = false,
                CreatedAtUtc = DateTime.UtcNow,
                ProcessedAt = null,
                HandledById = null,
                InternalNote = null,
                AppointmentId = null
            };

            _ctx.RegistrationRequests.Add(reg);

     
            var existingPatient = await _ctx.Patients.FirstOrDefaultAsync(p => p.Email == reg.Email, ct);
            if (existingPatient == null)
            {
      
                var randomPwd = Guid.NewGuid().ToString("N")[..8];
                var hash = BCrypt.Net.BCrypt.HashPassword(randomPwd);

                var newPatient = new Patient
                {
                    Email = reg.Email,
                    PasswordHash = hash,
                    FullName = reg.FullName,
                    Phone = reg.Phone,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _ctx.Patients.Add(newPatient);

      
                var subject = "Thông tin đăng ký khám tại Clinic";
                var body = $@"
                    <p>Xin chào {reg.FullName},</p>
                    <p>Bạn đã đăng ký khám thành công. Đây là thông tin tài khoản:</p>
                    <ul>
                        <li>Email: {reg.Email}</li>
                        <li>Password: <b>{randomPwd}</b></li>
                        <li>Ngày mong muốn khám: {reg.StartDate:dd/MM/yyyy}</li>
                    </ul>
                    <p>Vui lòng đăng nhập và đổi mật khẩu sau khi đăng nhập lần đầu.</p>
                    <p>Trân trọng,<br/>Clinic Team</p>
                ";

                await _email.SendEmailAsync(reg.Email, subject, body);
            }
            else
            {
             
                var subject = "Xác nhận đăng ký khám tại Clinic";
                var body = $@"
                    <p>Xin chào {existingPatient.FullName},</p>
                    <p>Clinic đã nhận được yêu cầu đăng ký khám của bạn.</p>
                    <ul>
                        <li>Email: {existingPatient.Email}</li>
                        <li>Ngày mong muốn khám: {reg.StartDate:dd/MM/yyyy}</li>
                    </ul>
                    <p>Nhân viên tư vấn sẽ liên hệ để xác nhận lịch khám và hướng dẫn thanh toán.</p>
                    <p>Trân trọng,<br/>Clinic Team</p>
                ";

                await _email.SendEmailAsync(existingPatient.Email, subject, body);
            }


            await _ctx.SaveChangesAsync(ct);

            return ServiceResult<int>.Ok(
                reg.RegistrationRequestId,
                "Đăng ký khám thành công. Vui lòng kiểm tra email để nhận thông tin chi tiết."
            );
        }
    }
}
