using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/employee/auth")]
    public class EmployeeAuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public EmployeeAuthController(IAuthService auth)
        {
            _auth = auth;
        }

      
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _auth.LoginEmployeeAsync(req, ct);
            if (result is null) return Unauthorized("Invalid credentials.");
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] RegisterEmployeeRequest req, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _auth.RegisterStaffAsync(req, userId, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "Staff_Doctor")]
        [HttpPost("create-doctor")]
        public async Task<IActionResult> CreateDoctor([FromBody] RegisterEmployeeRequest req, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _auth.RegisterDoctorAsync(req, userId, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
