using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Prescription;
using ClinicManagement.Application.Interfaces.Prescription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Prescription
{
    [ApiController]
    [Route("api/prescriptions")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _service;

        public PrescriptionController(IPrescriptionService service)
        {
            _service = service;
        }

        // 🔹 1. Danh sách bệnh nhân đã khám (Examined)
        [HttpGet("examined-patients")]
        public async Task<IActionResult> GetExaminedPatients([FromQuery] string? keyword = null)
        {
            var result = await _service.GetExaminedPatientsAsync(keyword);
            return Ok(result);
        }

        // 🔹 2. Lấy toàn bộ đơn thuốc (filter theo bác sĩ / bệnh nhân)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? doctorId = null,
            [FromQuery] int? patientId = null)
        {
            var result = await _service.GetAllPrescriptionsAsync(doctorId, patientId);
            return Ok(result);
        }

        // 🔹 3. Xem chi tiết 1 đơn thuốc
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetPrescriptionDetailAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

     
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PrescriptionRequestDto request,
            [FromQuery] int staffId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResult<string>.Fail("Dữ liệu không hợp lệ."));

            var result = await _service.CreatePrescriptionAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // . Cập nhật đơn thuốc 
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] PrescriptionRequestDto request,
            [FromQuery] int staffId)
        {
            var result = await _service.UpdatePrescriptionAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // 🔹 6. Xoá đơn thuốc 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePrescriptionAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // 🔹 7. Gửi email tóm tắt đơn thuốc cho bệnh nhân (nếu cần re-send)
        [HttpPost("{id}/send-email")]
        public async Task<IActionResult> SendEmail(int id)
        {
            var result = await _service.SendPrescriptionEmailAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
