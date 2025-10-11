using ClinicManagement.Application.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Dashboard { 
[ApiController]
[Route("api/staff-patient")]
public class StaffPatientController : ControllerBase
{
    private readonly IStaffPatientService _service;

    public StaffPatientController(IStaffPatientService service)
    {
        _service = service;
    }

    [HttpGet("registrations")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllRequestsAsync();
        return Ok(result);
    }

    [HttpGet("registrations/{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _service.GetRequestDetailAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("registrations/{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status, [FromQuery] int staffId)
    {
        var result = await _service.UpdateStatusAsync(id, status, staffId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("registrations/{id}/note")]
    public async Task<IActionResult> AddNote(int id, [FromQuery] int staffId, [FromBody] string note)
    {
        var result = await _service.AddNoteAsync(id, staffId, note);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
}
