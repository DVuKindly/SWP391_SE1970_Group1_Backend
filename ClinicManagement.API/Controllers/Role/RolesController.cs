using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Controllers.Role
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly ClinicDbContext _ctx;

        public RolesController(RoleService roleService, ClinicDbContext ctx)
        {
            _roleService = roleService;
            _ctx = ctx;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequest req, CancellationToken ct)
        {
            var result = await _roleService.CreateRoleAsync(req.Name, req.Description, ct);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet("GetAllstaff")]
        public async Task<IActionResult> GetStaffRoles(CancellationToken ct)
        {
           
            var staffRoles = await _ctx.Roles
                .Where(r => r.Name.StartsWith("Staff_"))
                .ToListAsync(ct);

            return Ok(staffRoles);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest req, CancellationToken ct)
        {
            var result = await _roleService.UpdateRoleAsync(id, req, ct);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

    }

    public class RoleCreateRequest
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }

}
