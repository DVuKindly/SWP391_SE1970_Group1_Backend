    using ClinicManagement.Application;
    using ClinicManagement.Application.DTOS.Request.Prescription;
    using ClinicManagement.Application.Interfaces.Prescription;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace ClinicManagement.API.Controllers.Prescription
    {
        [ApiController]
        [Route("api/doctor/prescriptions")]
        [Authorize(Roles = "Doctor")]
        public class PrescriptionController : ControllerBase
        {
            private readonly IPrescriptionService _service;

            public PrescriptionController(IPrescriptionService service)
            {
                _service = service;
            }

            // 🔹 Lấy doctorId từ JWT (Bearer token)
            private int GetDoctorId() =>
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 🔹 1. Danh sách đơn thuốc của bác sĩ
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var doctorId = GetDoctorId();
                var result = await _service.GetAllPrescriptionsForDoctorAsync(doctorId);
                return Ok(result);
            }

            // 🔹 2. Xem chi tiết 1 đơn thuốc (thuộc bệnh nhân của bác sĩ)
            [HttpGet("{id}")]
            public async Task<IActionResult> GetDetail(int id)
            {
                var doctorId = GetDoctorId();
                var result = await _service.GetPrescriptionDetailForDoctorAsync(id, doctorId);
                return result.Success ? Ok(result) : Unauthorized(result);
            }

            // 🔹 3. Kê đơn thuốc mới (chỉ cho bệnh nhân mà bác sĩ có cuộc hẹn)
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] PrescriptionRequestDto request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ServiceResult<string>.Fail("Dữ liệu không hợp lệ."));

                var doctorId = GetDoctorId();
                var result = await _service.CreatePrescriptionAsync(request, doctorId);
                return result.Success ? Ok(result) : BadRequest(result);
            }

            // 🔹 4. Cập nhật đơn thuốc
            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] PrescriptionRequestDto request)
            {
                var doctorId = GetDoctorId();
                var result = await _service.UpdatePrescriptionForDoctorAsync(id, request, doctorId);
                return result.Success ? Ok(result) : Unauthorized(result);
            }

            // 🔹 5. Xoá đơn thuốc
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var doctorId = GetDoctorId();
                var result = await _service.DeletePrescriptionForDoctorAsync(id, doctorId);
                return result.Success ? Ok(result) : Unauthorized(result);
            }

            // 🔹 6. Gửi lại email đơn thuốc cho bệnh nhân
            [HttpPost("{id}/send-email")]
            public async Task<IActionResult> SendEmail(int id)
            {
                var doctorId = GetDoctorId();
                var result = await _service.SendPrescriptionEmailForDoctorAsync(id, doctorId);
                return result.Success ? Ok(result) : BadRequest(result);
            }
        // 🔹 7. Danh sách bệnh nhân đã khám (chỉ của bác sĩ đang đăng nhập)
        [HttpGet("examined-patients")]
        public async Task<IActionResult> GetExaminedPatients([FromQuery] string? keyword = null)
        {
            var doctorId = GetDoctorId();
            var result = await _service.GetExaminedPatientsForDoctorAsync(doctorId, keyword);
            return Ok(result);
        }

    }
}
