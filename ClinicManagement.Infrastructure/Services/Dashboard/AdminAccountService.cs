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

       
        // 1. GetAccounts full íist
   
        public async Task<PagedResult<AccountDto>> GetAccountsAsync(
            string? role,
            string? keyword,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            IQueryable<AccountDto> query;

            if (role == "Patient")
            {
                query = _ctx.Patients.Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Role = "Patient",
                    Name = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                });
            }
            else if (!string.IsNullOrEmpty(role))
            {
                query = from e in _ctx.Employees
                        join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                        join r in _ctx.Roles on er.RoleId equals r.RoleId
                        where r.Name == role
                        select new AccountDto
                        {
                            Id = e.EmployeeUserId,
                            Role = r.Name,
                            Name = e.FullName,
                            Email = e.Email,
                            Phone = e.Phone,
                            IsActive = e.IsActive
                        };
            }
            else
            {
                // Nếu không filter role -> lấy tất cả
                query = from e in _ctx.Employees
                        join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                        join r in _ctx.Roles on er.RoleId equals r.RoleId
                        select new AccountDto
                        {
                            Id = e.EmployeeUserId,
                            Role = r.Name,
                            Name = e.FullName,
                            Email = e.Email,
                            Phone = e.Phone,
                            IsActive = e.IsActive
                        };

                query = query.Concat(
                    _ctx.Patients.Select(p => new AccountDto
                    {
                        Id = p.PatientUserId,
                        Role = "Patient",
                        Name = p.FullName,
                        Email = p.Email,
                        Phone = p.Phone,
                        IsActive = p.IsActive
                    })
                );
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                    x.Email.Contains(keyword) ||
                    x.Phone.Contains(keyword) ||
                    x.Name.Contains(keyword));
            }

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<AccountDto>
            {
                Items = items,
                TotalItems = total,
                Page = page,
                PageSize = pageSize
            };
        }


        // 2. GetAccountById

        public async Task<AccountDto?> GetAccountByEmailAsync(string email, CancellationToken ct)
        {
            email = email.Trim().ToLower();

       
            var emp = await (from e in _ctx.Employees
                             join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                             join r in _ctx.Roles on er.RoleId equals r.RoleId
                             where e.Email.ToLower() == email
                             select new AccountDto
                             {
                                 Id = e.EmployeeUserId,
                                 Role = r.Name,
                                 Name = e.FullName,
                                 Email = e.Email,
                                 Phone = e.Phone,
                                 IsActive = e.IsActive
                             }).FirstOrDefaultAsync(ct);

            if (emp != null) return emp;

        
            return await _ctx.Patients
                .Where(p => p.Email.ToLower() == email)
                .Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Role = "Patient",
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
    }
}
