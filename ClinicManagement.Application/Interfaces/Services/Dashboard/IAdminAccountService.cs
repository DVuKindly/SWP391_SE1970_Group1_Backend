using ClinicManagement.Application.DTOS.Request.Dashboard;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Services.Dashboard
{
    public interface IAdminAccountService
    {
        /// <summary>
        /// Lấy danh sách account theo role, keyword tìm kiếm, phân trang.
        /// </summary>
        Task<PagedResult<AccountDto>> GetAccountsAsync(
            string? role,
            string? keyword,
            int page,
            int pageSize,
            CancellationToken ct);

        Task<AccountDto?> GetAccountByEmailAsync(string email, CancellationToken ct);


        Task<bool> UpdateAccountStatusAsync(int accountId, bool isActive, CancellationToken ct);

        Task<int> BulkUpdateAccountStatusAsync(IEnumerable<int> accountIds, bool isActive, CancellationToken ct);


        Task<List<RoleDto>> GetAllRolesAsync(CancellationToken ct);
    }
}
