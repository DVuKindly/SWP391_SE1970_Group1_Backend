using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Request.Dashboard.Staff;
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
    public class StaffAccountService : IStaffAccountService
    {
        private readonly ClinicDbContext _ctx;

        public StaffAccountService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PagedResult<AccountDto>> GetAccountsAsync(
            string staffRole, string? keyword, int page, int pageSize, CancellationToken ct)
        {
            IQueryable<AccountDto> query;

            if (staffRole == "Staff_Patient")
            {
                query = _ctx.Patients.Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Roles = new List<string> { "Patient" },
                    Name = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                });
            }
            else if (staffRole == "Staff_Doctor")
            {
                query = from e in _ctx.Employees
                        join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                        join r in _ctx.Roles on er.RoleId equals r.RoleId
                        where r.Name == "Doctor"
                        select new AccountDto
                        {
                            Id = e.EmployeeUserId,
                            Roles = new List<string> { r.Name },
                            Name = e.FullName,
                            Email = e.Email,
                            Phone = e.Phone,
                            IsActive = e.IsActive
                        };
            }
            else
            {
                throw new UnauthorizedAccessException();
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
                .OrderBy(x => x.Name)
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

        public async Task<AccountDto?> GetAccountByEmailAsync(string email, string staffRole, CancellationToken ct)
        {
            email = email.Trim().ToLower();

            if (staffRole == "Staff_Patient")
            {
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
            else if (staffRole == "Staff_Doctor")
            {
                return await (from e in _ctx.Employees
                              join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                              join r in _ctx.Roles on er.RoleId equals r.RoleId
                              where e.Email.ToLower() == email && r.Name == "Doctor"
                              select new AccountDto
                              {
                                  Id = e.EmployeeUserId,
                                  Roles = new List<string> { r.Name },
                                  Name = e.FullName,
                                  Email = e.Email,
                                  Phone = e.Phone,
                                  IsActive = e.IsActive
                              })
                              .FirstOrDefaultAsync(ct);
            }

            throw new UnauthorizedAccessException();
        }

        public async Task<bool> UpdateAccountStatusAsync(int accountId, string staffRole, bool isActive, CancellationToken ct)
        {
            if (staffRole == "Staff_Patient")
            {
                var patient = await _ctx.Patients.FindAsync(new object[] { accountId }, ct);
                if (patient != null)
                {
                    patient.IsActive = isActive;
                    patient.UpdatedAtUtc = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync(ct);
                    return true;
                }
            }
            else if (staffRole == "Staff_Doctor")
            {
                var emp = await (from e in _ctx.Employees
                                 join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                                 join r in _ctx.Roles on er.RoleId equals r.RoleId
                                 where e.EmployeeUserId == accountId && r.Name == "Doctor"
                                 select e)
                                 .FirstOrDefaultAsync(ct);

                if (emp != null)
                {
                    emp.IsActive = isActive;
                    emp.UpdatedAtUtc = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync(ct);
                    return true;
                }
            }

            return false;
        }

        public async Task<int> BulkUpdateAccountStatusAsync(IEnumerable<int> accountIds, string staffRole, bool isActive, CancellationToken ct)
        {
            int updated = 0;

            if (staffRole == "Staff_Patient")
            {
                var patients = await _ctx.Patients
                    .Where(p => accountIds.Contains(p.PatientUserId))
                    .ToListAsync(ct);

                foreach (var p in patients)
                {
                    p.IsActive = isActive;
                    p.UpdatedAtUtc = DateTime.UtcNow;
                }

                updated = patients.Count;
            }
            else if (staffRole == "Staff_Doctor")
            {
                var emps = await (from e in _ctx.Employees
                                  join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                                  join r in _ctx.Roles on er.RoleId equals r.RoleId
                                  where accountIds.Contains(e.EmployeeUserId) && r.Name == "Doctor"
                                  select e)
                                  .ToListAsync(ct);

                foreach (var e in emps)
                {
                    e.IsActive = isActive;
                    e.UpdatedAtUtc = DateTime.UtcNow;
                }

                updated = emps.Count;
            }

            if (updated > 0)
            {
                await _ctx.SaveChangesAsync(ct);
            }

            return updated;
        }

        public async Task<bool> ResetPasswordAsync(int accountId, string staffRole, string newPassword, CancellationToken ct)
        {
            string hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (staffRole == "Staff_Patient")
            {
                var patient = await _ctx.Patients.FindAsync(new object[] { accountId }, ct);
                if (patient != null)
                {
                    patient.PasswordHash = hashed;
                    patient.UpdatedAtUtc = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync(ct);
                    return true;
                }
            }
            else if (staffRole == "Staff_Doctor")
            {
                var emp = await (from e in _ctx.Employees
                                 join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                                 join r in _ctx.Roles on er.RoleId equals r.RoleId
                                 where e.EmployeeUserId == accountId && r.Name == "Doctor"
                                 select e)
                                 .FirstOrDefaultAsync(ct);

                if (emp != null)
                {
                    emp.PasswordHash = hashed;
                    emp.UpdatedAtUtc = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync(ct);
                    return true;
                }
            }

            return false;
        }

        public async Task<List<AccountDto>> FilterAccountsByStatusAsync(string staffRole, bool isActive, CancellationToken ct)
        {
            if (staffRole == "Staff_Patient")
            {
                return await _ctx.Patients
                    .Where(p => p.IsActive == isActive)
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
            }
            else if (staffRole == "Staff_Doctor")
            {
                return await (from e in _ctx.Employees
                              join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                              join r in _ctx.Roles on er.RoleId equals r.RoleId
                              where r.Name == "Doctor" && e.IsActive == isActive
                              select new AccountDto
                              {
                                  Id = e.EmployeeUserId,
                                  Roles = new List<string> { r.Name },
                                  Name = e.FullName,
                                  Email = e.Email,
                                  Phone = e.Phone,
                                  IsActive = e.IsActive
                              })
                              .ToListAsync(ct);
            }

            throw new UnauthorizedAccessException();
        }

        public async Task<StaffProfileDto?> GetMyProfileAsync(int staffId, CancellationToken ct)
        {
            return await _ctx.Employees
                .Where(e => e.EmployeeUserId == staffId)
                .Select(e => new StaffProfileDto
                {
                    StaffId = e.EmployeeUserId,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Image = e.Image,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> UpdateMyProfileAsync(int staffId, UpdateStaffProfileRequest req, CancellationToken ct)
        {
            var staff = await _ctx.Employees.FindAsync(new object[] { staffId }, ct);
            if (staff == null) return false;

            if (!string.IsNullOrEmpty(req.FullName)) staff.FullName = req.FullName;
            if (!string.IsNullOrEmpty(req.Phone)) staff.Phone = req.Phone;
            if (!string.IsNullOrEmpty(req.Image)) staff.Image = req.Image;

            staff.UpdatedAtUtc = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);
            return true;
        }
    }
}
