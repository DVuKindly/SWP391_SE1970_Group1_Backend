using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
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

        // doctor
        public async Task<AuthResponse?> LoginDoctorAsync(LoginRequest req, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            var doctor = await _ctx.Doctors
                .FirstOrDefaultAsync(d => d.Email == email && d.IsActive, ct);
            if (doctor is null) return null;

            if (!BCrypt.Net.BCrypt.Verify(req.Password, doctor.PasswordHash)) return null;

            var tokens = GenerateTokens(doctor.DoctorId.ToString(), "Doctor");

            doctor.RefreshToken = tokens.RefreshToken;
            doctor.RefreshTokenExpiry = tokens.RefreshExpiry;
            doctor.LastLoginAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = "Doctor",
                AccountId = doctor.DoctorId 
            };
        }

        // patient
        public async Task<AuthResponse?> LoginPatientAsync(LoginRequest req, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            var patient = await _ctx.Patients
                .FirstOrDefaultAsync(p => p.Email == email && p.IsActive, ct);
            if (patient is null) return null;

            if (!BCrypt.Net.BCrypt.Verify(req.Password, patient.PasswordHash)) return null;

            var tokens = GenerateTokens(patient.PatientId.ToString(), "Patient");

            patient.RefreshToken = tokens.RefreshToken;
            patient.RefreshTokenExpiry = tokens.RefreshExpiry;
            patient.LastLoginAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = "Patient",
                AccountId = patient.PatientId
            };
        }

        // staff
        public async Task<AuthResponse?> LoginStaffAsync(LoginRequest req, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            var staff = await _ctx.Staffs
                .FirstOrDefaultAsync(s => s.Email == email && s.IsActive, ct);
            if (staff is null) return null;

            if (!BCrypt.Net.BCrypt.Verify(req.Password, staff.PasswordHash)) return null;

            var tokens = GenerateTokens(staff.StaffId.ToString(), "Staff");

            staff.RefreshToken = tokens.RefreshToken;
            staff.RefreshTokenExpiry = tokens.RefreshExpiry;
            staff.LastLoginAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = "Staff",
                AccountId = staff.StaffId
            };
        }

        // admin
        public async Task<AuthResponse?> LoginAdminAsync(LoginRequest req, CancellationToken ct = default)
        {
            var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
            // Admin chính là Staff có cờ IsAdmin
            var admin = await _ctx.Admins
                .FirstOrDefaultAsync(s => s.Email == email && s.IsActive, ct);
            if (admin is null) return null;

            if (!BCrypt.Net.BCrypt.Verify(req.Password, admin.PasswordHash)) return null;

            var tokens = GenerateTokens(admin.AdminId.ToString(), "Admin");

            admin.RefreshToken = tokens.RefreshToken;
            admin.RefreshTokenExpiry = tokens.RefreshExpiry;
            admin.LastLoginAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);

            return new AuthResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.AccessExpiry,
                Role = "Admin",
                AccountId = admin.AdminId
            };
        }

       
        private (string AccessToken, DateTime AccessExpiry, string RefreshToken, DateTime RefreshExpiry)
            GenerateTokens(string userId, string role)
        {
            var issuer = _cfg["Jwt:Issuer"]!;
            var audience = _cfg["Jwt:Audience"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(_cfg["Jwt:AccessTokenMinutes"] ?? "60")
            );

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: accessExpiry,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshExp = DateTime.UtcNow.AddDays(
                int.Parse(_cfg["Jwt:RefreshTokenDays"] ?? "7")
            );

            return (accessToken, accessExpiry, refreshToken, refreshExp);
        }
    }
}
