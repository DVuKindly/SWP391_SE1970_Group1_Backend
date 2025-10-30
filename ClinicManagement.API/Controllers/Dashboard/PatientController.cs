using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Dashboard.Patient;

using ClinicManagement.Application.Interfaces.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers.Dashboard
{
    [ApiController]
    [Route("api/patient")]
    [Authorize(Roles = "Patient")] // chỉ bệnh nhân mới được truy cập
    public class PatientController : ControllerBase
    {
        private readonly IPatientServices _patientService;

        public PatientController(IPatientServices patientService)
        {
            _patientService = patientService;
        }

        // 🔸 Helper lấy ID từ token
        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        // =============================
        // 🧩 1️⃣ HỒ SƠ CÁ NHÂN
        // =============================

        /// <summary>
        /// Lấy thông tin hồ sơ của chính bệnh nhân đang đăng nhập
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int patientId = GetUserId();
            var result = await _patientService.GetProfileAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cập nhật hồ sơ cá nhân (bệnh nhân)
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileDto dto)
        {
            int patientId = GetUserId();
            var result = await _patientService.UpdateProfileAsync(patientId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // =============================
        // 📅 2️⃣ LỊCH HẸN CỦA TÔI
        // =============================

        /// <summary>
        /// Danh sách lịch hẹn của bệnh nhân đang đăng nhập
        /// </summary>
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            int patientId = GetUserId();
            var result = await _patientService.GetMyAppointmentsAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xem chi tiết 1 lịch hẹn (bảo đảm là của bệnh nhân này)
        /// </summary>
        [HttpGet("appointment-detail/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentDetail(int appointmentId)
        {
            var result = await _patientService.GetAppointmentDetailAsync(appointmentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // =============================
        // 💊 3️⃣ ĐƠN THUỐC
        // =============================

        /// <summary>
        /// Danh sách đơn thuốc của bệnh nhân đang đăng nhập
        /// </summary>
        [HttpGet("prescriptions")]
        public async Task<IActionResult> GetPrescriptions()
        {
            int patientId = GetUserId();
            var result = await _patientService.GetMyPrescriptionsAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Chi tiết 1 đơn thuốc (chỉ xem được nếu thuộc bệnh nhân này)
        /// </summary>
        [HttpGet("prescription-detail/{prescriptionId}")]
        public async Task<IActionResult> GetPrescriptionDetail(int prescriptionId)
        {
            var result = await _patientService.GetPrescriptionDetailAsync(prescriptionId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

   
        //[HttpGet("summary")]
        //public async Task<IActionResult> GetDashboardSummary()
        //{
        //    int patientId = GetUserId();
        //    var result = await _patientService.GetPatientDashboardSummaryAsync(patientId);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}
    }
}
