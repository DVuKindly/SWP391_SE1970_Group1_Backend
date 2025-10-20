using ClinicManagement.Application.Interfaces.Booking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Exam
{
    [ApiController]
    [Route("api/exams")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _service;

        public ExamController(IExamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var result = await _service.GetAllExamsAsync(includeInactive);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetExamDetailAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
