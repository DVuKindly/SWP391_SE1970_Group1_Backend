using ClinicManagement.Application.DTOS.Request.Dashboard.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Dashboard
{
    public interface IPatientServices
    {
        // 1️⃣ Hồ sơ cá nhân
        Task<ServiceResult<PatientProfileDto>> GetProfileAsync(int patientId);
        Task<ServiceResult<bool>> UpdateProfileAsync(int patientId, UpdatePatientProfileDto dto);

        // 2️⃣ Danh sách lịch hẹn (Appointments)
        Task<ServiceResult<List<PatientAppointmentDto>>> GetMyAppointmentsAsync(int patientId);
        Task<ServiceResult<PatientAppointmentDto>> GetAppointmentDetailAsync(int appointmentId);

        // 3️⃣ Danh sách đơn thuốc
        Task<ServiceResult<List<PatientPrescriptionDto>>> GetMyPrescriptionsAsync(int patientId);
        Task<ServiceResult<PatientPrescriptionDto>> GetPrescriptionDetailAsync(int prescriptionId);
    }
}
