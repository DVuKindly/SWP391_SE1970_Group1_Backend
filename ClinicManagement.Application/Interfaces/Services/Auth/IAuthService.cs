using ClinicManagement.Application.DTOS.Common;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
using System.Security.Claims;
using System.Threading;

namespace ClinicManagement.Application.Interfaces.Services.Auth
{
    public interface IAuthService
    {
        // Đăng nhập
        Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct = default);

        // Refresh token (dùng bearer + refresh token)
        Task<AuthResponse?> RefreshAsync(ClaimsPrincipal user, RefreshRequest req, CancellationToken ct = default);

        // Logout (dùng bearer + refresh token)
        Task<bool> LogoutAsync(ClaimsPrincipal user, string refreshToken, CancellationToken ct = default);

        // Đổi mật khẩu (dùng bearer)
        Task<bool> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordRequest req, CancellationToken ct = default);

        // Đăng ký bệnh nhân (public)
        Task<ServiceResult<AuthResponse>> RegisterPatientAsync(RegisterPatientRequest req, CancellationToken ct = default);

        // Admin tạo account (ít dùng vì đã có CreateStaffByAdminAsync)
        Task<bool> AdminCreateAccountAsync(AdminCreateAccountRequest req, CancellationToken ct = default);

        // Nhân viên tạo Staff
        Task<CreateStaffResult?> CreateStaffByAdminAsync(CreateStaffAccountRequest req, CancellationToken ct = default);

        // Nhân viên (Staff_Doctor) tạo Doctor
        Task<CreateDoctorResult?> CreateDoctorByStaffAsync(CreateDoctorMinimalRequest req, int creatorStaffAccountId, CancellationToken ct = default);

        // Nhân viên (Staff_Patient) tạo Patient
        Task<CreatePatientResult?> CreatePatientByStaffAsync(CreatePatientMinimalRequest req, int creatorStaffAccountId, CancellationToken ct = default);
    }
}
