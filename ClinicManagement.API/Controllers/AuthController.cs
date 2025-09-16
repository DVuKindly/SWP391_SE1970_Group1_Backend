using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);
            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(result);
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshAsync(request, ct);
            if (result == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            return Ok(result);
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken ct)
        {
            var result = await _authService.LogoutAsync(request.AccountId, request.RefreshToken, ct);
            if (!result)
                return BadRequest(new { message = "Logout failed" });

            return Ok(new { message = "Logged out successfully" });
        }

        // POST: api/auth/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var result = await _authService.ChangePasswordAsync(request, ct);
            if (!result)
                return BadRequest(new { message = "Change password failed" });

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterPatientRequest req, CancellationToken ct)
        {
            var res = await _authService.RegisterPatientAsync(req, ct);
            if (res is null) return BadRequest(new { message = "Email already exists or role missing." });
            return Ok(res); // trả luôn token
        }

        // Admin/Staff tạo account (không trả token)
        [HttpPost("admin/create-account")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AdminCreateAccount([FromBody] AdminCreateAccountRequest req, CancellationToken ct)
        {
            var ok = await _authService.AdminCreateAccountAsync(req, ct);
            return ok ? Ok(new { message = "Account created." }) : BadRequest(new { message = "Create failed." });
        }
    }
}
