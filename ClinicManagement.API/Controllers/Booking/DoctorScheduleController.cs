using ClinicManagement.Application.Interfaces.Booking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Booking
{
    [ApiController]
    [Route("api/doctor-schedule")]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IDoctorScheduleService _doctorScheduleService;

        public DoctorScheduleController(IDoctorScheduleService doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }

        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctorSchedule(int doctorId, DateTime? from, DateTime? to)
        {
            var result = await _doctorScheduleService.GetDoctorScheduleAsync(doctorId, from, to);
            return Ok(result);
        }

        [HttpGet("{doctorId}/work-patterns")]
        public async Task<IActionResult> GetWorkPatterns(int doctorId)
        {
            var result = await _doctorScheduleService.GetWorkPatternsAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("{doctorId}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
        {
            if (date == default)
                return BadRequest("Date là bắt buộc.");

            var result = await _doctorScheduleService.GetAvailableSlotsAsync(doctorId, date);
            return Ok(result);
        }
    }

}
