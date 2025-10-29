using ClinicManagement.Application.DTOS.Request.Appointment;
using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Response.Appoitment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Appoiment
{
    public interface IAppointmentService
    {
        Task<ServiceResult<List<AppointmentResponseDto>>> GetAllAppointmentsAsync(string? status = null);
        Task<ServiceResult<AppointmentResponseDto>> GetAppointmentDetailAsync(int id);
        Task<ServiceResult<AppointmentResponseDto>> CreateAppointmentAsync(AppointmentRequestDto request, int createdById);
        Task<ServiceResult<bool>> ApproveAppointmentAsync(int id, int approvedById, bool approve);
        Task<ServiceResult<bool>> DeleteAppointmentAsync(int id);
        Task<ServiceResult<List<EligiblePatientResponseDto>>> GetEligiblePatientsAsync();
        Task<ServiceResult<List<DoctorScheduleResponseDto>>> GetAllDoctorsWithWorkPatternsAsync();

        Task<ServiceResult<List<AppointmentResponseDto>>> GetAllAppointmentsAsync(string? status = null, string? keyword = null);

    }
}
