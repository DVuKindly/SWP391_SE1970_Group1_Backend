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
    [ApiController]
    [Route("api/staff/accounts")]
    [Authorize(Roles = "Staff_Patient,Staff_Doctor")]
    public class StaffAccountController : ControllerBase
    {
        private readonly IStaffAccountService _service;

        public StaffAccountController(IStaffAccountService service)
        {
            _service = service;
        }

        private string GetStaffRole()
        {
            if (User.IsInRole("Staff_Patient")) return "Staff_Patient";
            if (User.IsInRole("Staff_Doctor")) return "Staff_Doctor";
            throw new UnauthorizedAccessException();
        }

        private int GetCurrentStaffId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

  
        [HttpGet]
        public async Task<IActionResult> GetAccounts(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var role = GetStaffRole();
            var result = await _service.GetAccountsAsync(role, keyword, page, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetAccountByEmail(
            [FromQuery] string email,
            CancellationToken ct)
        {
            var role = GetStaffRole();
            var account = await _service.GetAccountByEmailAsync(email, role, ct);
            if (account == null) return NotFound();
            return Ok(account);
        }

 
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateAccountStatus(
            int id,
            [FromQuery] bool isActive,
            CancellationToken ct)
        {
            var role = GetStaffRole();
            var success = await _service.UpdateAccountStatusAsync(id, role, isActive, ct);
            if (!success) return Forbid();
            return NoContent();
        }

  
        [HttpPut("bulk/Editstatusmany")]
        public async Task<IActionResult> BulkUpdateStatus(
            [FromBody] BulkUpdateStatusRequest req,
            CancellationToken ct)
        {
            var role = GetStaffRole();
            var updated = await _service.BulkUpdateAccountStatusAsync(req.AccountIds, role, req.IsActive, ct);
            return Ok(new { Updated = updated });
        }

   
        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int id,
            [FromQuery] string newPassword,
            CancellationToken ct)
        {
            var role = GetStaffRole();
            var success = await _service.ResetPasswordAsync(id, role, newPassword, ct);
            if (!success) return Forbid();
            return NoContent();
        }

    
        [HttpGet("filterByStatusaccount")]
        public async Task<IActionResult> FilterAccounts(
            [FromQuery] bool isActive,
            CancellationToken ct)
        {
            var role = GetStaffRole();
            var accounts = await _service.FilterAccountsByStatusAsync(role, isActive, ct);
            return Ok(accounts);
        }

        [HttpGet("GetProfileme")]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            var staffId = GetCurrentStaffId();
            var profile = await _service.GetMyProfileAsync(staffId, ct);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut("UpdateProfileme")]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateStaffProfileRequest req,
            CancellationToken ct)
        {
            var staffId = GetCurrentStaffId();
            var success = await _service.UpdateMyProfileAsync(staffId, req, ct);
            if (!success) return NotFound();
            return NoContent();
        }
    }


    public class BulkUpdateStatusRequest
    {
        public IEnumerable<int> AccountIds { get; set; } = new List<int>();
        public bool IsActive { get; set; }
    }
}
