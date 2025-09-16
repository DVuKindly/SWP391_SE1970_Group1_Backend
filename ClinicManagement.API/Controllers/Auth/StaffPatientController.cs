using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/staff-patient")]
    [Authorize(Roles = "Staff_Patient")]
    public class StaffPatientController : ControllerBase
    {
        private readonly IAuthService _auth;

        public StaffPatientController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientMinimalRequest req, CancellationToken ct)
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? "0";
            int staffAccountId = int.TryParse(sub, out var id) ? id : 0;

            var result = await _auth.CreatePatientByStaffAsync(req, staffAccountId, ct);
            if (result is null) return Conflict(new { message = "Email already exists." });
            return Ok(result);
        }
    }
}
