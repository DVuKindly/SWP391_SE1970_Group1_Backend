using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Request.Dashboard.Staff;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Dashboard
{
    public interface IStaffAccountService
    {
   
        Task<PagedResult<AccountDto>> GetAccountsAsync(
            string staffRole,
            string? keyword,
            int page,
            int pageSize,
            CancellationToken ct);

    
        Task<AccountDto?> GetAccountByEmailAsync(
            string email,
            string staffRole,
            CancellationToken ct);

      
        Task<bool> UpdateAccountStatusAsync(
            int accountId,
            string staffRole,
            bool isActive,
            CancellationToken ct);

        Task<int> BulkUpdateAccountStatusAsync(
            IEnumerable<int> accountIds,
            string staffRole,
            bool isActive,
            CancellationToken ct);

       
        Task<bool> ResetPasswordAsync(
            int accountId,
            string staffRole,
            string newPassword,
            CancellationToken ct);


        Task<List<AccountDto>> FilterAccountsByStatusAsync(
            string staffRole,
            bool isActive,
            CancellationToken ct);

    
        Task<StaffProfileDto?> GetMyProfileAsync(
            int staffId,
            CancellationToken ct);

   
        Task<bool> UpdateMyProfileAsync(
            int staffId,
            UpdateStaffProfileRequest req,
            CancellationToken ct);
    }
}
