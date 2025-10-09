using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
using ClinicManagement.Application.Interfaces.JWT;
using ClinicManagement.Application.Interfaces.Services.Auth;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClinicManagement.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ClinicDbContext _ctx;
        private readonly IConfiguration _cfg;
        private readonly IJwtService _jwt;


        public AuthService(ClinicDbContext ctx, IConfiguration cfg, IJwtService jwt)
        {
            _ctx = ctx;
            _cfg = cfg;
            _jwt = jwt;
        }

        // login 


        // 1. Login bệnh nhân
        public async Task<ServiceResult<AuthResponse>> LoginPatientAsync(LoginRequest req, CancellationToken ct = default)
        {
            try
            {
                var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
                var pwd = req.Password ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pwd))
                    return ServiceResult<AuthResponse>.Fail("Email và mật khẩu là bắt buộc.");

                if (!new EmailAddressAttribute().IsValid(email))
                    return ServiceResult<AuthResponse>.Fail("Email không đúng định dạng.");

                var patient = await _ctx.Patients.FirstOrDefaultAsync(p => p.Email == email, ct);
                if (patient is null)
                    return ServiceResult<AuthResponse>.Fail("Tài khoản bệnh nhân không tồn tại.");

                if (!patient.IsActive)
                    return ServiceResult<AuthResponse>.Fail("Tài khoản đã bị khóa. Vui lòng liên hệ hỗ trợ.");

                if (!BCrypt.Net.BCrypt.Verify(pwd, patient.PasswordHash))
                    return ServiceResult<AuthResponse>.Fail("Mật khẩu không chính xác.");

                var claims = new List<Claim>
        {
            new Claim("sub", patient.PatientUserId.ToString()),
            new Claim(ClaimTypes.Email, patient.Email),
            new Claim(ClaimTypes.Name, patient.FullName),
            new Claim(ClaimTypes.Role, "Patient")
        };

                var (accessToken, refreshToken) = _jwt.GenerateTokens(claims);


                patient.RefreshToken = refreshToken;
                patient.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                patient.LastLoginAtUtc = DateTime.UtcNow;
                await _ctx.SaveChangesAsync(ct);

                var payload = new AuthResponse
                {
                    UserId = patient.PatientUserId,
                    Email = patient.Email,
                    FullName = patient.FullName,
                    Roles = new[] { "Patient" },
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                return ServiceResult<AuthResponse>.Ok(payload, "Đăng nhập bệnh nhân thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponse>.Fail($"Lỗi đăng nhập: {ex.Message}");
            }
        }
        // google login 
        public async Task<ServiceResult<AuthResponse>> GoogleLoginPatientAsync(GoogleLoginRequest req, CancellationToken ct = default)
        {
            try
            {

                var payload = await GoogleJsonWebSignature.ValidateAsync(req.IdToken);
                if (payload == null)
                    return ServiceResult<AuthResponse>.Fail("Google token không hợp lệ.");


                var email = (payload.Email ?? string.Empty).Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(email))
                    return ServiceResult<AuthResponse>.Fail("Google không trả về email.");

                // 3. Tìm bệnh nhân theo email
                var patient = await _ctx.Patients.FirstOrDefaultAsync(p => p.Email == email, ct);

                // 4. Nếu chưa có thì auto-register
                if (patient is null)
                {
                    patient = new Patient
                    {
                        Email = email,
                        FullName = string.IsNullOrWhiteSpace(payload.Name) ? email.Split('@')[0] : payload.Name,
                        Phone = "", // tránh null nếu DB đang NOT NULL
                        PasswordHash = Guid.NewGuid().ToString("N"), // tránh null (chỉ login Google)
                        IsActive = true,
                        CreatedAtUtc = DateTime.UtcNow
                    };

                    _ctx.Patients.Add(patient);
                    await _ctx.SaveChangesAsync(ct);
                }

                // 5. Check trạng thái tài khoản
                if (!patient.IsActive)
                    return ServiceResult<AuthResponse>.Fail("Tài khoản đã bị khóa. Vui lòng liên hệ hỗ trợ.");

                // 6. Claims để phát JWT
                var claims = new List<Claim>
        {
            new Claim("sub", patient.PatientUserId.ToString()),
            new Claim(ClaimTypes.Email, patient.Email),
            new Claim(ClaimTypes.Name, patient.FullName),
            new Claim(ClaimTypes.Role, "Patient")
        };

                var (accessToken, refreshToken) = _jwt.GenerateTokens(claims);


                // 7. Update Refresh Token
                patient.RefreshToken = refreshToken;
                patient.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                patient.LastLoginAtUtc = DateTime.UtcNow;
                await _ctx.SaveChangesAsync(ct);

                // 8. Trả kết quả
                var payloadRes = new AuthResponse
                {
                    UserId = patient.PatientUserId,
                    Email = patient.Email,
                    FullName = patient.FullName,
                    Roles = new[] { "Patient" },
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                return ServiceResult<AuthResponse>.Ok(payloadRes, "Đăng nhập Google thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponse>.Fail($"Google login error: {ex.Message}");
            }
        }

        //2 register
        public async Task<ServiceResult<AuthResponse>> RegisterPatientAsync(RegisterPatientRequest req, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (await _ctx.Patients.AnyAsync(p => p.Email == email, ct))
                return ServiceResult<AuthResponse>.Fail("Email đã được đăng ký.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var patient = new Patient
            {
                Email = email,
                PasswordHash = passwordHash,
                FullName = req.FullName.Trim(),
                Phone = req.Phone.Trim(),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _ctx.Patients.Add(patient);
            await _ctx.SaveChangesAsync(ct);

            var claims = new List<Claim>
    {
        new Claim("sub", patient.PatientUserId.ToString()),
        new Claim(ClaimTypes.Email, patient.Email),
        new Claim(ClaimTypes.Name, patient.FullName),
        new Claim(ClaimTypes.Role, "Patient")
    };

            var (accessToken, refreshToken) = _jwt.GenerateTokens(claims);


            patient.RefreshToken = refreshToken;
            patient.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            patient.LastLoginAtUtc = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);

            var payload = new AuthResponse
            {
                UserId = patient.PatientUserId,
                Email = patient.Email,
                FullName = patient.FullName,
                Roles = new[] { "Patient" },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return ServiceResult<AuthResponse>.Ok(payload, "Đăng ký bệnh nhân thành công.");
        }





        // employee
        public async Task<ServiceResult<AuthResponse>> LoginEmployeeAsync(LoginRequest req, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            var pwd = req.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pwd))
                return ServiceResult<AuthResponse>.Fail("Email và mật khẩu là bắt buộc.");

            if (!new EmailAddressAttribute().IsValid(email))
                return ServiceResult<AuthResponse>.Fail("Email không đúng định dạng.");

            var employee = await _ctx.Employees
                .Include(e => e.EmployeeRoles).ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Email == email, ct);

            if (employee is null)
                return ServiceResult<AuthResponse>.Fail("Tài khoản nhân sự không tồn tại.");

            if (!employee.IsActive)
                return ServiceResult<AuthResponse>.Fail("Tài khoản đã bị khóa. Vui lòng liên hệ quản trị.");

            if (!BCrypt.Net.BCrypt.Verify(pwd, employee.PasswordHash))
                return ServiceResult<AuthResponse>.Fail("Mật khẩu không chính xác.");

            var roles = employee.EmployeeRoles.Select(er => er.Role.Name).Distinct().ToArray();
            if (roles.Length == 0)
                return ServiceResult<AuthResponse>.Fail("Tài khoản chưa được gán vai trò.");

            var claims = new List<Claim>
    {
        new Claim("sub", employee.EmployeeUserId.ToString()),
        new Claim(ClaimTypes.Email, employee.Email),
        new Claim(ClaimTypes.Name, employee.FullName)
    };
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var (accessToken, refreshToken) = _jwt.GenerateTokens(claims);

            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            employee.LastLoginAtUtc = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);

            var payload = new AuthResponse
            {
                UserId = employee.EmployeeUserId,
                Email = employee.Email,
                FullName = employee.FullName,
                Roles = roles,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return ServiceResult<AuthResponse>.Ok(payload, "Đăng nhập nhân sự thành công.");
        }


        public async Task<ServiceResult<AuthResponse>> RegisterStaffAsync(RegisterEmployeeRequest req, int createdById, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (await _ctx.Employees.AnyAsync(e => e.Email == email, ct))
                return ServiceResult<AuthResponse>.Fail("Email đã tồn tại.");

            if (req.RoleName != "Staff_Patient" && req.RoleName != "Staff_Doctor")
                return ServiceResult<AuthResponse>.Fail("Admin chỉ được tạo Staff_Patient hoặc Staff_Doctor.");

            var role = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == req.RoleName, ct);
            if (role is null) return ServiceResult<AuthResponse>.Fail("Role không tồn tại.");

            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var employee = new Employee
            {
                Email = email,
                PasswordHash = hash,
                FullName = req.FullName.Trim(),
                Phone = req.Phone,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            _ctx.Employees.Add(employee);
            await _ctx.SaveChangesAsync(ct);

            var empRole = new EmployeeRole
            {
                EmployeeId = employee.EmployeeUserId,
                RoleId = role.RoleId,
                AssignedById = createdById,
                AssignedAtUtc = DateTime.UtcNow
            };
            _ctx.EmployeeRoles.Add(empRole);
            await _ctx.SaveChangesAsync(ct);

            return ServiceResult<AuthResponse>.Ok(new AuthResponse
            {
                UserId = employee.EmployeeUserId,
                Email = employee.Email,
                FullName = employee.FullName,
                Roles = new[] { req.RoleName }
            }, "Tạo tài khoản Staff thành công.");
        }

        public async Task<ServiceResult<AuthResponse>> RegisterDoctorAsync(
      CreateDoctorRequest req, int createdById, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (await _ctx.Employees.AnyAsync(e => e.Email == email, ct))
                return ServiceResult<AuthResponse>.Fail("Email đã tồn tại.");

            var role = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor", ct);
            if (role is null) return ServiceResult<AuthResponse>.Fail("Role Doctor chưa được seed.");

            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var employee = new Employee
            {
                Email = email,
                PasswordHash = hash,
                FullName = req.FullName.Trim(),
                Phone = req.Phone,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            _ctx.Employees.Add(employee);
            await _ctx.SaveChangesAsync(ct);

            var empRole = new EmployeeRole
            {
                EmployeeId = employee.EmployeeUserId,
                RoleId = role.RoleId,
                AssignedById = createdById,
                AssignedAtUtc = DateTime.UtcNow
            };
            _ctx.EmployeeRoles.Add(empRole);

            // DoctorProfile khởi tạo từ request
            var profile = new DoctorProfile
            {
                EmployeeId = employee.EmployeeUserId,
                Title = req.Title ?? "Bác sĩ",
                Biography = req.Biography,
                Degree = req.Degree,
                Education = req.Education,
                ExperienceYears = req.ExperienceYears,
                Certifications = req.Certifications,
                CreatedAtUtc = DateTime.UtcNow
            };
            _ctx.DoctorProfiles.Add(profile);

            // Mapping Departments
            foreach (var dep in req.Departments)
            {
                var doctorDep = new DoctorDepartment
                {
                    DoctorId = employee.EmployeeUserId,
                    DepartmentId = dep.DepartmentId,
                    IsPrimary = dep.IsPrimary,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _ctx.DoctorDepartments.Add(doctorDep);
            }

            await _ctx.SaveChangesAsync(ct);

            // ✅ GÁN LỊCH MẶC ĐỊNH CHO BÁC SĨ MỚI
            await AssignDefaultWorkPatternsToDoctorAsync(employee.EmployeeUserId, ct);

            return ServiceResult<AuthResponse>.Ok(new AuthResponse
            {
                UserId = employee.EmployeeUserId,
                Email = employee.Email,
                FullName = employee.FullName,
                Roles = new[] { "Doctor" }
            }, "Tạo tài khoản Doctor thành công.");
        }
        private async Task AssignDefaultWorkPatternsToDoctorAsync(int doctorId, CancellationToken ct = default)
        {
            // Lấy các template đang active
            var templates = await _ctx.WorkPatternTemplates
                .Where(t => t.IsActive)
                .ToListAsync(ct);

            if (!templates.Any()) return;

            var patterns = templates.Select(t => new DoctorWorkPattern
            {
                DoctorId = doctorId,
                DayOfWeek = t.DayOfWeek,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                SlotMinutes = t.SlotMinutes,
                CreatedAtUtc = DateTime.UtcNow
            }).ToList();

            _ctx.DoctorWorkPatterns.AddRange(patterns);
            await _ctx.SaveChangesAsync(ct);
        }





        // tạo account 2-3 role

        public async Task<ServiceResult<AuthResponse>> CreateAccountWithRolesAsync(
    CreateAccountRequest req,
    int createdById,
    CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();

            // Kiểm tra email đã tồn tại chưa
            if (await _ctx.Employees.AnyAsync(e => e.Email == email, ct))
                return ServiceResult<AuthResponse>.Fail("Email đã tồn tại.");

            if (req.RoleNames == null || req.RoleNames.Count == 0)
                return ServiceResult<AuthResponse>.Fail("Phải chọn ít nhất 1 vai trò.");

            // Lấy roles từ DB
            var roles = await _ctx.Roles
                .Where(r => req.RoleNames.Contains(r.Name))
                .ToListAsync(ct);

            if (roles.Count != req.RoleNames.Count)
                return ServiceResult<AuthResponse>.Fail("Một hoặc nhiều vai trò không hợp lệ.");

            // Tạo Employee
            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var employee = new Employee
            {
                Email = email,
                PasswordHash = hash,
                FullName = req.FullName.Trim(),
                Phone = req.Phone?.Trim(),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _ctx.Employees.Add(employee);
            await _ctx.SaveChangesAsync(ct);

            // Gán roles
            foreach (var role in roles)
            {
                _ctx.EmployeeRoles.Add(new EmployeeRole
                {
                    EmployeeId = employee.EmployeeUserId,
                    RoleId = role.RoleId,
                    AssignedById = createdById,
                    AssignedAtUtc = DateTime.UtcNow
                });
            }

            await _ctx.SaveChangesAsync(ct);

            return ServiceResult<AuthResponse>.Ok(new AuthResponse
            {
                UserId = employee.EmployeeUserId,
                Email = employee.Email,
                FullName = employee.FullName,
                Roles = roles.Select(r => r.Name).ToArray()
            }, "Tạo tài khoản với nhiều vai trò thành công.");
        }

    }
}
