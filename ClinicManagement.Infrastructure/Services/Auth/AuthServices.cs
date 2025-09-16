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

        public async Task<AuthResponse?> RefreshAsync(RefreshRequest req, CancellationToken ct = default)
        {
            var acc = await _ctx.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == req.AccountId && a.IsActive, ct);

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

        public async Task<bool> LogoutAsync(int accountId, string refreshToken, CancellationToken ct = default)
        {
            var acc = await _ctx.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId, ct);
            if (acc is null || acc.RefreshToken != refreshToken) return false;

            acc.RefreshToken = null;
            acc.RefreshTokenExpiry = null;
            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest req, CancellationToken ct = default)
        {
            var acc = await _ctx.Accounts.FirstOrDefaultAsync(a => a.AccountId == req.AccountId && a.IsActive, ct);
            if (acc is null) return false;

            if (!BCrypt.Net.BCrypt.Verify(req.CurrentPassword, acc.PasswordHash))
                return false;

            acc.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            acc.UpdatedAt = DateTime.UtcNow;

            // revoke refresh tokens sau khi đổi mật khẩu
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
                Address = req.Address,
                Gender = req.Gender,
                DOB = req.DOB,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.Patients.Add(patient);
            await _ctx.SaveChangesAsync(ct); // cần Id để link Account

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

            // Tạo token trả về
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
    }
}

