// ClinicManagement.Application.Interfaces.Services.Auth.IAuthService
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
using System.Security.Claims;

public interface IAuthService
{

    Task<AuthResponse?> LoginDoctorAsync(LoginRequest req, CancellationToken ct = default);
    Task<AuthResponse?> LoginPatientAsync(LoginRequest req, CancellationToken ct = default);
    Task<AuthResponse?> LoginStaffAsync(LoginRequest req, CancellationToken ct = default);
    Task<AuthResponse?> LoginAdminAsync(LoginRequest req, CancellationToken ct = default);


}
