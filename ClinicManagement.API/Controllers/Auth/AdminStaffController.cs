
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/admin/staffs")]
    [Authorize(Roles = "Admin")]
    public class AdminStaffController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AdminStaffController(IAuthService auth) => _auth = auth;

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffAccountRequest req, CancellationToken ct)
        {
            var result = await _auth.CreateStaffByAdminAsync(req, ct);
            if (result is null) return Conflict(new { message = "Email already exists." });
            return Ok(result);
        }
    }
}
