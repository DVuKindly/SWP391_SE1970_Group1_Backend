using ClinicManagement.Application;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Application.DTOS.Request.Auth;
using ClinicManagement.Application.DTOS.Request.Booking;
using ClinicManagement.Application.DTOS.Response.Auth;
using ClinicManagement.Application.DTOS.Request.Dashboard;

namespace ClinicManagement.Application.Interfaces.Services.Dashboard
{
    public interface IStaffPatientService
    {
        Task<ServiceResult<List<RegistrationRequestResponseDto>>> GetAllRequestsAsync(string? status = null);



        Task<ServiceResult<PagedResult<RegistrationRequestResponseDto>>> GetRequestsAsync(
           string? status = null,
           string? email = null,
           int page = 1,
           int pageSize = 10);
        Task<ServiceResult<RegistrationRequestDetailDto>> GetRequestDetailAsync(int requestId);

        Task<ServiceResult<string>> UpdateStatusAsync(int requestId, string newStatus, int staffId);

        Task<ServiceResult<string>> AddNoteAsync(int requestId, int staffId, string note);

        Task<ServiceResult<string>> MarkAsInvalidAsync(int requestId, int staffId, string reason);

 
        Task<ServiceResult<string>> ExecuteDirectPaymentAsync(int requestId, int staffId, int examId);
    }
}
