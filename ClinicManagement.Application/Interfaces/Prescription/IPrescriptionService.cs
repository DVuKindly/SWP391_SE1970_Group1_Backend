using ClinicManagement.Application.DTOS.Request.Prescription;
using ClinicManagement.Application.DTOS.Request.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Prescription
{
    public interface IPrescriptionService
    {
        // 🔹 1. Lấy danh sách tất cả đơn thuốc của bác sĩ (theo doctorId)
        Task<ServiceResult<List<PrescriptionResponseDto>>> GetAllPrescriptionsForDoctorAsync(int doctorId);

<<<<<<< HEAD
        // 🔹 2. Xem chi tiết 1 đơn thuốc (chỉ nếu đơn đó thuộc bệnh nhân của bác sĩ)
        Task<ServiceResult<PrescriptionResponseDto>> GetPrescriptionDetailForDoctorAsync(int id, int doctorId);

        // 🔹 3. Kê đơn thuốc mới (chỉ cho bệnh nhân mà bác sĩ có Appointment)
        Task<ServiceResult<PrescriptionResponseDto>> CreatePrescriptionAsync(PrescriptionRequestDto req, int doctorId);

        // 🔹 4. Cập nhật đơn thuốc (chỉ khi đơn đó thuộc bác sĩ)
        Task<ServiceResult<PrescriptionResponseDto>> UpdatePrescriptionForDoctorAsync(int id, PrescriptionRequestDto req, int doctorId);

        // 🔹 5. Xoá đơn thuốc (chỉ khi đơn đó thuộc bác sĩ)
        Task<ServiceResult<bool>> DeletePrescriptionForDoctorAsync(int id, int doctorId);

        // 🔹 6. Gửi lại email đơn thuốc (chỉ khi đơn đó thuộc bác sĩ)
        Task<ServiceResult<string>> SendPrescriptionEmailForDoctorAsync(int id, int doctorId);

        // 🔹 7. (Tuỳ chọn) Tổng hợp thống kê thanh toán nếu sau này cần
=======
        Task<ServiceResult<PrescriptionResponseDto>> CreatePrescriptionAsync(PrescriptionRequestDto request);
        Task<ServiceResult<PrescriptionResponseDto>> UpdatePrescriptionAsync(int id, PrescriptionRequestDto request);
        Task<ServiceResult<bool>> DeletePrescriptionAsync(int id);
        Task<ServiceResult<string>> SendPrescriptionEmailAsync(int id);
>>>>>>> edc0bd1dc47ff96986fe08bc40d44b78f6a5ebea
        Task<ServiceResult<PatientPaymentSummaryDto>> GetPaymentSummaryAsync(DateTime? from = null, DateTime? to = null);
    }
}
