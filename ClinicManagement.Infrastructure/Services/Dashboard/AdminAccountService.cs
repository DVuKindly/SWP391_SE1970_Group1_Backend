using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Dashboard
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly ClinicDbContext _ctx;

        public AdminAccountService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        // 1. GetAccounts full list
        public async Task<PagedResult<AccountDto>> GetAccountsAsync(
            string? role,
            string? keyword,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            // 1. Employees với nhiều role -> 1 record
            var employees = await (
                from e in _ctx.Employees
                join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                join r in _ctx.Roles on er.RoleId equals r.RoleId
                group r by new { e.EmployeeUserId, e.FullName, e.Email, e.Phone, e.IsActive } into g
                select new AccountDto
                {
                    Id = g.Key.EmployeeUserId,
                    Roles = g.Select(x => x.Name).ToList(),
                    Name = g.Key.FullName,
                    Email = g.Key.Email,
                    Phone = g.Key.Phone,
                    IsActive = g.Key.IsActive
                }).ToListAsync(ct);

            // 2. Patients (chỉ có 1 role "Patient")
            var patients = await _ctx.Patients
                .Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Roles = new List<string> { "Patient" },
                    Name = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                })
                .ToListAsync(ct);

            // 3. Combine employees + patients (LINQ to Objects, không lỗi EF)
            var combined = employees.Concat(patients).AsQueryable();

            // 4. Lọc theo role
            if (!string.IsNullOrEmpty(role))
            {
                combined = combined.Where(a => a.Roles.Contains(role));
            }

            // 5. Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                combined = combined.Where(x =>
                    x.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            // 6. Tổng số record
            var total = combined.Count();

            // 7. Phân trang
            var items = combined
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<AccountDto>
            {
                Items = items,
                TotalItems = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // 2. GetAccountByEmail
        public async Task<AccountDto?> GetAccountByEmailAsync(string email, CancellationToken ct)
        {
            email = email.Trim().ToLower();

            var emp = await (from e in _ctx.Employees
                             join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                             join r in _ctx.Roles on er.RoleId equals r.RoleId
                             where e.Email.ToLower() == email
                             group r by new { e.EmployeeUserId, e.FullName, e.Email, e.Phone, e.IsActive } into g
                             select new AccountDto
                             {
                                 Id = g.Key.EmployeeUserId,
                                 Roles = g.Select(x => x.Name).ToList(),
                                 Name = g.Key.FullName,
                                 Email = g.Key.Email,
                                 Phone = g.Key.Phone,
                                 IsActive = g.Key.IsActive
                             }).FirstOrDefaultAsync(ct);

            if (emp != null) return emp;

            return await _ctx.Patients
                .Where(p => p.Email.ToLower() == email)
                .Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Roles = new List<string> { "Patient" },
                    Name = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync(ct);
        }

        // 3. Update status (lock/unlock)
        public async Task<bool> UpdateAccountStatusAsync(int accountId, bool isActive, CancellationToken ct)
        {
            var emp = await _ctx.Employees.FindAsync(new object[] { accountId }, ct);
            if (emp != null)
            {
                emp.IsActive = isActive;
                emp.UpdatedAtUtc = DateTime.UtcNow;
                await _ctx.SaveChangesAsync(ct);
                return true;
            }

            var patient = await _ctx.Patients.FindAsync(new object[] { accountId }, ct);
            if (patient != null)
            {
                patient.IsActive = isActive;
                patient.UpdatedAtUtc = DateTime.UtcNow;
                await _ctx.SaveChangesAsync(ct);
                return true;
            }

            return false;
        }

        // 4. Bulk update status
        public async Task<int> BulkUpdateAccountStatusAsync(IEnumerable<int> accountIds, bool isActive, CancellationToken ct)
        {
            int updated = 0;

            var emps = await _ctx.Employees.Where(e => accountIds.Contains(e.EmployeeUserId)).ToListAsync(ct);
            foreach (var e in emps)
            {
                e.IsActive = isActive;
                e.UpdatedAtUtc = DateTime.UtcNow;
            }
            updated += emps.Count;

            var patients = await _ctx.Patients.Where(p => accountIds.Contains(p.PatientUserId)).ToListAsync(ct);
            foreach (var p in patients)
            {
                p.IsActive = isActive;
                p.UpdatedAtUtc = DateTime.UtcNow;
            }
            updated += patients.Count;

            if (updated > 0)
            {
                await _ctx.SaveChangesAsync(ct);
            }

            return updated;
        }

        // 5. Get all roles
        public async Task<List<RoleDto>> GetAllRolesAsync(CancellationToken ct)
        {
            return await _ctx.Roles
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync(ct);
        }

        // 6. Update profile
        public async Task<bool> UpdateProfileAsync(int accountId, UpdateProfileDtoAdmin dto, CancellationToken ct)
        {
            // 1. Tìm trong Employees trước
            var emp = await _ctx.Employees.FindAsync(new object[] { accountId }, ct);
            if (emp != null)
            {
                emp.Email = dto.Email.Trim().ToLower();
                emp.Phone = dto.Phone.Trim();
                emp.FullName = dto.FullName.Trim();
                emp.UpdatedAtUtc = DateTime.UtcNow;
                await _ctx.SaveChangesAsync(ct);
                return true;
            }

            // 2. Nếu không có thì tìm trong Patients
            var patient = await _ctx.Patients.FindAsync(new object[] { accountId }, ct);
            if (patient != null)
            {
                patient.Email = dto.Email.Trim().ToLower();
                patient.Phone = dto.Phone.Trim();
                patient.FullName = dto.FullName.Trim();
                patient.UpdatedAtUtc = DateTime.UtcNow;
                await _ctx.SaveChangesAsync(ct);
                return true;
            }

            return false;
        }
    }
}
