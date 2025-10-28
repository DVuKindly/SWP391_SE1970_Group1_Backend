using ClinicManagement.Application;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers.Dashboard {
    [ApiController]
    [Route("api/staff-patient")]
    [Authorize(Roles = "Staff_Patient,Admin")] // ✅ chỉ staff có quyền
    public class StaffPatientController : ControllerBase
    {
        private readonly IStaffPatientService _service;

        public StaffPatientController(IStaffPatientService service)
        {
            _service = service;
        }

        // Lấy ID từ JWT Claims
        private int GetStaffId()
        {
            var claim = User.FindFirst("staffId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpGet("registrations")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var result = await _service.GetAllRequestsAsync(status);
            return Ok(result);
        }

        [HttpGet("registrations/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetRequestDetailAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("registrations/{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            int staffId = GetStaffId();
            var result = await _service.UpdateStatusAsync(id, status, staffId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("registrations/{id}/note")]
        public async Task<IActionResult> AddNote(int id, [FromBody] string note)
        {
            int staffId = GetStaffId(); 
            var result = await _service.AddNoteAsync(id, staffId, note);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("registrations/{id}/invalid")]
        public async Task<IActionResult> MarkInvalid(int id, [FromBody] string? reason)
        {
            int staffId = GetStaffId();
            var result = await _service.MarkAsInvalidAsync(id, staffId, reason ?? "Không xác minh được thông tin.");
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPut("registrations/{id}/direct-payment")]
        public async Task<IActionResult> ExecuteDirectPayment(int id, [FromQuery] int examId)
        {
        
            int staffId = GetStaffId();

      
            if (examId <= 0)
                return BadRequest(ServiceResult<string>.Fail("Vui lòng chọn gói khám hợp lệ."));

      
            var result = await _service.ExecuteDirectPaymentAsync(id, staffId, examId);

 
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }


    }
}
