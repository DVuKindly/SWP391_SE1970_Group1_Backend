using ClinicManagement.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Application.DTOS.Request.Auth;

namespace ClinicManagement.API.Controllers.Auth
{
    [ApiController]
    [Route("api/staff-doctor")]
 
    public class StaffDoctorController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly AppDbContext _ctx;

        public StaffDoctorController(IAuthService auth, AppDbContext ctx)
        {
            _auth = auth;
            _ctx = ctx;
        }
        [Authorize(Roles = "Staff_Doctor")]
        [HttpPost("doctors")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorMinimalRequest req, CancellationToken ct)
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? "0";
            int staffAccountId = int.TryParse(sub, out var id) ? id : 0;

            var result = await _auth.CreateDoctorByStaffAsync(req, staffAccountId, ct);
            if (result is null) return Conflict(new { message = "Email already exists." });
            return Ok(result);
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetAllDepartments(CancellationToken ct)
        {
            var depts = await _ctx.Departments
                .Where(d => d.IsActive)
                .Select(d => new
                {
                    d.DepartmentId,
                    d.Code,
                    d.Name
                })
                .ToListAsync(ct);

            return Ok(depts);
        }
    }
}
