using ClinicManagement.Application.DTOS.Request.Appointment;
using ClinicManagement.Application.Interfaces.Appoiment;
using ClinicManagement.Infrastructure.Services.Appoiment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Appointment
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var result = await _service.GetAllAppointmentsAsync(status);
            return Ok(result);
        }

     
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetAppointmentDetailAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        
        [HttpGet("eligible-patients")]
        public async Task<IActionResult> GetEligiblePatients()
        {
            var result = await _service.GetEligiblePatientsAsync();
            return Ok(result);
        }

   
        [HttpGet("scheduledoctors")]
        public async Task<IActionResult> GetDoctorsWithSchedules()
        {
            var result = await _service.GetAllDoctorsWithWorkPatternsAsync();
            return Ok(result);
        }

      
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentRequestDto request)
        {
            if (request == null)
                return BadRequest("Yêu cầu không hợp lệ.");


            int createdById = 1;

            var result = await _service.CreateAppointmentAsync(request, createdById);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromQuery] bool approve = true)
        {
            // TODO: sau này lấy approvedById từ JWT
            int approvedById = 1;

            var result = await _service.ApproveAppointmentAsync(id, approvedById, approve);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAppointmentAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        [HttpGet("appointments_Filter")]
        public async Task<IActionResult> GetAllAppointments(
    [FromQuery] string? status,
    [FromQuery] string? keyword)
        {
            var result = await _service.GetAllAppointmentsAsync(status, keyword);
            return Ok(result);
        }


    }
}
