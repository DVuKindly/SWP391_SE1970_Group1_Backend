using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/patient/auth")]
    public class PatientAuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public PatientAuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _auth.LoginPatientAsync(req, ct);
            if (result is null) return Unauthorized("Invalid credentials.");
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPatientRequest req, CancellationToken ct)
        {
            var result = await _auth.RegisterPatientAsync(req, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest req, CancellationToken ct)
        {
            var result = await _auth.GoogleLoginPatientAsync(req, ct);
            if (result is null) return Unauthorized("Invalid Google token.");
            return Ok(result);
        }


    }
}
