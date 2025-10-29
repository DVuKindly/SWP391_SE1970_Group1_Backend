using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Exam;
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
        //thêm 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExamRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<string>.Fail("Dữ liệu không hợp lệ."));

            var result = await _service.CreateExamAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        //  Xóa 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool softDelete = true)
        {
            var result = await _service.DeleteExamAsync(id, softDelete);
            return result.Success ? Ok(result) : NotFound(result);
        }
        //  Cập nhật
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExamRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<string>.Fail("Dữ liệu không hợp lệ."));

            var result = await _service.UpdateExamAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
