using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using ClinicManagement.Domain.Entity;                
using ClinicManagement.Infrastructure.Persistence;   
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ClinicManagement.Infrastructure.Services.Auth
{

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _cfg;

        public AuthService(AppDbContext ctx, IConfiguration cfg)
        {
            _ctx = ctx;
            _cfg = cfg;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct = default)
        {
            var acc = await _ctx.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == req.Email && a.IsActive, ct);

            if (acc is null) return null;
            if (!BCrypt.Net.BCrypt.Verify(req.Password, acc.PasswordHash)) return null;

            var tokens = GenerateTokens(acc);
            acc.RefreshToken = tokens.RefreshToken;
            acc.RefreshTokenExpiry = tokens.RefreshExpiry;
            acc.LastLoginAt = DateTime.UtcNow;

            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = acc.Role.Name,
                AccountId = acc.AccountId
            };
        }

        public async Task<AuthResponse?> RefreshAsync(
     ClaimsPrincipal user,
     RefreshRequest req,
     CancellationToken ct = default)
        {
            var accountId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);


            var acc = await _ctx.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.IsActive, ct);

            if (acc is null) return null;
            if (acc.RefreshToken != req.RefreshToken) return null;
            if (acc.RefreshTokenExpiry is null || acc.RefreshTokenExpiry <= DateTime.UtcNow) return null;

            var tokens = GenerateTokens(acc);
            acc.RefreshToken = tokens.RefreshToken;
            acc.RefreshTokenExpiry = tokens.RefreshExpiry;

            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = acc.Role.Name,
                AccountId = acc.AccountId
            };
        }


        public async Task<bool> LogoutAsync(
        ClaimsPrincipal user,
        string refreshToken,
        CancellationToken ct = default)
        {
            var accountId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);


            var acc = await _ctx.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId, ct);
            if (acc is null || acc.RefreshToken != refreshToken) return false;

            acc.RefreshToken = null;
            acc.RefreshTokenExpiry = null;
            await _ctx.SaveChangesAsync(ct);
            return true;
        }


        public async Task<bool> ChangePasswordAsync(
            ClaimsPrincipal user,
            ChangePasswordRequest req,
            CancellationToken ct = default)
        {
            var accountId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);


            var acc = await _ctx.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId && a.IsActive, ct);
            if (acc is null) return false;

            if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, acc.PasswordHash))
                return false;

            acc.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            acc.UpdatedAt = DateTime.UtcNow;
            acc.RefreshToken = null;
            acc.RefreshTokenExpiry = null;

            await _ctx.SaveChangesAsync(ct);
            return true;
        }



        private (string AccessToken, DateTime AccessExpiry, string RefreshToken, DateTime RefreshExpiry)
            GenerateTokens(Account acc)
        {
            var issuer = _cfg["Jwt:Issuer"]!;
            var audience = _cfg["Jwt:Audience"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:AccessTokenMinutes"]!));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, acc.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, acc.Email),
                new Claim(ClaimTypes.Role, acc.Role.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwt = new JwtSecurityToken(issuer, audience, claims,
                                           expires: accessExpiry, signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshExp = DateTime.UtcNow.AddDays(int.Parse(_cfg["Jwt:RefreshTokenDays"]!));

            return (accessToken, accessExpiry, refreshToken, refreshExp);
        }



        public async Task<AuthResponse?> RegisterPatientAsync(RegisterPatientRequest req, CancellationToken ct = default)
        {
            // Email đã tồn tại?
            var existed = await _ctx.Accounts.AnyAsync(a => a.Email == req.Email, ct);
            if (existed) return null;

            // Lấy role Patient
            var rolePatient = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Patient", ct);
            if (rolePatient is null) throw new InvalidOperationException("Role 'Patient' not found. Seed roles first.");

            // Tạo Patient
            var patient = new Patient
            {
                Name = req.Name,
                Email = req.Email,
                Phone = req.Phone,
                 IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Patients.Add(patient);
            await _ctx.SaveChangesAsync(ct);

            // Tạo Account
            var acc = new Account
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = rolePatient.RoleId,
                PatientId = patient.PatientId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Accounts.Add(acc);
            await _ctx.SaveChangesAsync(ct);

          
            acc = await _ctx.Accounts.Include(a => a.Role).FirstAsync(a => a.AccountId == acc.AccountId, ct);
            var tokens = GenerateTokens(acc);
            acc.RefreshToken = tokens.RefreshToken;
            acc.RefreshTokenExpiry = tokens.RefreshExpiry;
            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = acc.Role.Name,
                AccountId = acc.AccountId
            };
        }

        public async Task<bool> AdminCreateAccountAsync(AdminCreateAccountRequest req, CancellationToken ct = default)
        {
            // Kiểm tra email trùng
            if (await _ctx.Accounts.AnyAsync(a => a.Email == req.Email, ct)) return false;

            // Lấy role theo tên
            var role = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == req.RoleName, ct);
            if (role is null) return false;

            // Ràng buộc link 1 trong 3 (giống CK_Accounts_OneLink)
            var linkCount = (req.DoctorId.HasValue ? 1 : 0) + (req.PatientId.HasValue ? 1 : 0) + (req.StaffId.HasValue ? 1 : 0);
            if (linkCount > 1) return false;

            var acc = new Account
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = role.RoleId,
                DoctorId = req.DoctorId,
                PatientId = req.PatientId,
                StaffId = req.StaffId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _ctx.Accounts.Add(acc);
            await _ctx.SaveChangesAsync(ct);
            return true;
        }




        public async Task<CreateDoctorResult?> CreateDoctorByStaffAsync(
          CreateDoctorMinimalRequest req,
          int creatorStaffAccountId,
          CancellationToken ct = default)
        {
            // 1) Validate
            if (req.Departments == null || req.Departments.Count == 0)
                throw new ArgumentException("At least one department is required.");
            var primaryCount = req.Departments.Count(d => d.IsPrimary);
            if (primaryCount != 1)
                throw new ArgumentException("Exactly one primary department is required.");

            // 2) Email chưa tồn tại?
            if (await _ctx.Accounts.AnyAsync(a => a.Email == req.Email, ct))
                return null;

            // 3) Lấy role Doctor
            var roleDoctor = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor", ct)
                             ?? throw new InvalidOperationException("Role 'Doctor' not found.");

            // 4) Kiểm tra departments tồn tại & active
            var deptIds = req.Departments.Select(d => d.DepartmentId).Distinct().ToList();
            var existed = await _ctx.Departments
                .Where(d => deptIds.Contains(d.DepartmentId) && d.IsActive)
                .Select(d => d.DepartmentId)
                .ToListAsync(ct);
            if (existed.Count != deptIds.Count)
                throw new ArgumentException("Some departments do not exist or are inactive.");

            // 5) Tạo Doctor (tối giản)
            var local = req.Email.Split('@')[0];
            var doctor = new Doctor
            {
                Email = req.Email,
                Name = string.IsNullOrWhiteSpace(local) ? "New Doctor" : local,
                Phone = "", // placeholder
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Doctors.Add(doctor);
            await _ctx.SaveChangesAsync(ct); // lấy DoctorId

            // 6) Map DoctorDepartments
            var docDepts = req.Departments.Select(d => new DoctorDepartment
            {
                DoctorId = doctor.DoctorId,
                DepartmentId = d.DepartmentId,
                IsPrimary = d.IsPrimary,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            _ctx.DoctorDepartments.AddRange(docDepts);

            // 7) Tạo Account cho Doctor
            var acc = new Account
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = roleDoctor.RoleId,
                DoctorId = doctor.DoctorId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Accounts.Add(acc);

            await _ctx.SaveChangesAsync(ct);

            // 8) Lấy lại Doctor kèm Departments để map response
            var result = await _ctx.Doctors
                .Include(d => d.DoctorDepartments)
                    .ThenInclude(dd => dd.Department)
                .FirstAsync(d => d.DoctorId == doctor.DoctorId, ct);

            return new CreateDoctorResult
            {
                DoctorId = result.DoctorId,
                Email = result.Email,
             
                Departments = result.DoctorDepartments.Select(dd => new DepartmentDto
                {
                    DepartmentId = dd.DepartmentId,
                    Code = dd.Department.Code,
                    Name = dd.Department.Name,
                    IsPrimary = dd.IsPrimary
                }).ToList()
            };
        }







        public async Task<CreateStaffResult?> CreateStaffByAdminAsync(CreateStaffAccountRequest req, CancellationToken ct = default)
        {
            // Validate role name cho an toàn (không cho tạo role khác)
            var allowed = new[] { "Staff_Doctor", "Staff_Patient" };
            if (!allowed.Contains(req.StaffRoleName))
                throw new ArgumentException("StaffRoleName must be 'Staff_Doctor' or 'Staff_Patient'.");

            // Email unique?
            if (await _ctx.Accounts.AnyAsync(a => a.Email == req.Email, ct))
                return null;

            // Tìm role theo Name (không dùng RoleId)
            var role = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == req.StaffRoleName, ct)
                       ?? throw new InvalidOperationException($"Role '{req.StaffRoleName}' not found.");

            // Tạo Staff
            var staff = new Staff
            {
                Email = req.Email,
                Name = req.Name,
                Phone = req.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Staffs.Add(staff);
            await _ctx.SaveChangesAsync(ct);

            // Tạo Account gắn Staff
            var acc = new Account
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = role.RoleId,
                StaffId = staff.StaffId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Accounts.Add(acc);
            await _ctx.SaveChangesAsync(ct);

            return new CreateStaffResult
            {
                StaffId = staff.StaffId,
                AccountId = acc.AccountId,
                Email = acc.Email,
                Role = role.Name
            };
        }

       

        public async Task<CreatePatientResult?> CreatePatientByStaffAsync(
            CreatePatientMinimalRequest req,
            int creatorStaffAccountId,
            CancellationToken ct = default)
        {
            if (await _ctx.Accounts.AnyAsync(a => a.Email == req.Email, ct))
                return null;

            var rolePatient = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == "Patient", ct)
                              ?? throw new InvalidOperationException("Role 'Patient' not found.");

            var local = req.Email.Split('@')[0];
            var patient = new Patient
            {
                Email = req.Email,
                Name = string.IsNullOrWhiteSpace(local) ? "New Patient" : local,
                Phone = "", // placeholder
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Patients.Add(patient);
            await _ctx.SaveChangesAsync(ct);

            var acc = new Account
            {
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = rolePatient.RoleId,
                PatientId = patient.PatientId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Accounts.Add(acc);
            await _ctx.SaveChangesAsync(ct);

            return new CreatePatientResult
            {
                PatientId = patient.PatientId,
                AccountId = acc.AccountId,
                Email = acc.Email
            };
        }
    }

}


