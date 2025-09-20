using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.API.Controllers.Dashboard
{
    [ApiController]
    [Route("api/admin/accounts")]
    [Authorize(Roles = "Admin")]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAdminAccountService _service;

        public AdminAccountController(IAdminAccountService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAccounts(
            [FromQuery] string? role,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _service.GetAccountsAsync(role, keyword, page, pageSize, ct);
            return Ok(result);
        }


        [HttpGet("by-email")]
        public async Task<IActionResult> GetAccountByEmail([FromQuery] string email, CancellationToken ct)
        {
            var account = await _service.GetAccountByEmailAsync(email, ct);
            if (account == null) return NotFound();
            return Ok(account);
        }



        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateAccountStatus(
            int id,
            [FromQuery] bool isActive,
            CancellationToken ct)
        {
            var success = await _service.UpdateAccountStatusAsync(id, isActive, ct);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("bulk/status")]
        public async Task<IActionResult> BulkUpdateStatus(
            [FromBody] BulkStatusUpdateRequest request,
            CancellationToken ct)
        {
            var updated = await _service.BulkUpdateAccountStatusAsync(request.AccountIds, request.IsActive, ct);
            return Ok(new { UpdatedCount = updated });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles(CancellationToken ct)
        {
            var roles = await _service.GetAllRolesAsync(ct);
            return Ok(roles);
        }
    }

 
    public class BulkStatusUpdateRequest
    {
        public List<int> AccountIds { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
