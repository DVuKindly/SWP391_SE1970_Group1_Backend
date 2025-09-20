using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Request.Dashboard.Staff;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.API.Controllers.Dashboard
{
    [Route("api/doctor/accounts")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class DoctorAccountController : ControllerBase
    {
        private readonly IDoctorAccountService _service;

        public DoctorAccountController(IDoctorAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _service.GetPatientsAsync(keyword, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetPatientByEmail(
            [FromQuery] string email,
            CancellationToken ct = default)
        {
            var patient = await _service.GetPatientByEmailAsync(email, ct);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

    
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdatePatientStatus(
            int id,
            [FromQuery] bool isActive,
            CancellationToken ct = default)
        {
            var success = await _service.UpdatePatientStatusAsync(id, isActive, ct);
            if (!success) return NotFound();
            return Ok(new { Message = "Patient status updated successfully" });
        }

   
        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPatientPassword(
            int id,
            [FromBody] string newPassword,
            CancellationToken ct = default)
        {
            var success = await _service.ResetPatientPasswordAsync(id, newPassword, ct);
            if (!success) return NotFound();
            return Ok(new { Message = "Password reset successfully" });
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("GetProfileme")]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct = default)
        {
            // Lấy doctorId từ JWT claim
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorIdClaim == null) return Unauthorized();

            if (!int.TryParse(doctorIdClaim, out var doctorId))
                return Unauthorized();

            var profile = await _service.GetMyProfileAsync(doctorId, ct);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateProfileme")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateStaffProfileRequest req,
            CancellationToken ct = default)
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorIdClaim == null) return Unauthorized();

            if (!int.TryParse(doctorIdClaim, out var doctorId))
                return Unauthorized();

            var success = await _service.UpdateMyProfileAsync(doctorId, req, ct);
            if (!success) return BadRequest();
            return Ok(new { Message = "Profile updated successfully" });
        }
    }
}
