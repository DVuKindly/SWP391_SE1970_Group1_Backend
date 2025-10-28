using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Department;
using ClinicManagement.Application.Interfaces.Department;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Department
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var result = await _service.GetAllDepartmentsAsync(includeInactive);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetDepartmentByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<string>.Fail("Dữ liệu không hợp lệ."));

            var result = await _service.CreateDepartmentAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpdateDto request)
        {
            if (id != request.DepartmentId)
                return BadRequest(ServiceResult<string>.Fail("ID không khớp."));

            var result = await _service.UpdateDepartmentAsync(request);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool softDelete = true)
        {
            var result = await _service.DeleteDepartmentAsync(id, softDelete);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
