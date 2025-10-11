
using ClinicManagement.Application;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Request.Booking;
using ClinicManagement.Application.DTOS.Response.Auth;

namespace ClinicManagement.Application.Interfaces.Services.Dashboard
{
    public interface IStaffPatientService
    {

        Task<ServiceResult<List<RegistrationRequestResponseDto>>> GetAllRequestsAsync();



        Task<ServiceResult<RegistrationRequestDetailDto>> GetRequestDetailAsync(int requestId);

  
        Task<ServiceResult<string>> UpdateStatusAsync(int requestId, string newStatus, int staffId);

   
        Task<ServiceResult<string>> AddNoteAsync(int requestId, int staffId, string note);
    }
}
