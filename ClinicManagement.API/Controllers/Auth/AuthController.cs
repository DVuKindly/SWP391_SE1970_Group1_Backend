using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers.Auth
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);
            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshAsync(User, request, ct);
            if (result == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken ct)
        {
            var result = await _authService.LogoutAsync(User, request.RefreshToken, ct);
            if (!result)
                return BadRequest(new { message = "Logout failed" });

            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var result = await _authService.ChangePasswordAsync(User, request, ct);
            if (!result)
                return BadRequest(new { message = "Change password failed" });

            return Ok(new { message = "Password changed successfully" });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPatientRequest req, CancellationToken ct)
        {
            var res = await _authService.RegisterPatientAsync(req, ct);
            if (res is null)
                return BadRequest(new { message = "Email already exists or role missing." });

            return Ok(res);
        }

        //[Authorize(Roles = "Admin,Staff")]
        //[HttpPost("admin/create-account")]
        //public async Task<IActionResult> AdminCreateAccount([FromBody] AdminCreateAccountRequest req, CancellationToken ct)
        //{
        //    var ok = await _authService.AdminCreateAccountAsync(req, ct);
        //    if (!ok)
        //        return BadRequest(new { message = "Create failed." });

        //    return Ok(new { message = "Account created." });
        //}
    }
}
