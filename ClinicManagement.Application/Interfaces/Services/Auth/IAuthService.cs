using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Response.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct = default);
        Task<AuthResponse?> RefreshAsync(RefreshRequest req, CancellationToken ct = default);
        Task<bool> LogoutAsync(int accountId, string refreshToken, CancellationToken ct = default);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest req, CancellationToken ct = default);



        // NEW:
        Task<AuthResponse?> RegisterPatientAsync(RegisterPatientRequest req, CancellationToken ct = default);
        Task<bool> AdminCreateAccountAsync(AdminCreateAccountRequest req, CancellationToken ct = default);
    }
}
