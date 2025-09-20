using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Request.Dashboard.Doctor;
using ClinicManagement.Application.DTOS.Request.Dashboard.Staff; 
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Dashboard
{
    public interface IDoctorAccountService
    {
        Task<PagedResult<AccountDto>> GetPatientsAsync(
            string? keyword,
            int page,
            int pageSize,
            CancellationToken ct);

        Task<AccountDto?> GetPatientByEmailAsync(
            string email,
            CancellationToken ct);

        Task<bool> UpdatePatientStatusAsync(
            int patientId,
            bool isActive,
            CancellationToken ct);

        Task<int> BulkUpdatePatientStatusAsync(
            IEnumerable<int> patientIds,
            bool isActive,
            CancellationToken ct);

        Task<bool> ResetPatientPasswordAsync(
            int patientId,
            string newPassword,
            CancellationToken ct);

        Task<List<AccountDto>> FilterPatientsByStatusAsync(
            bool isActive,
            CancellationToken ct);

        Task<DoctorProfileDto?> GetMyProfileAsync(
            int doctorId,
            CancellationToken ct);

        Task<bool> UpdateMyProfileAsync(
            int doctorId,
            UpdateStaffProfileRequest req,
            CancellationToken ct);
    }
}
