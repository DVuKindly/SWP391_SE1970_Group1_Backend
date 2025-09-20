using ClinicManagement.Application.DTOS.Request.Dashboard;
using ClinicManagement.Application.DTOS.Request.Dashboard.Doctor;
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
    public class DoctorAccountService : IDoctorAccountService
    {
        private readonly ClinicDbContext _ctx;

        public DoctorAccountService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PagedResult<AccountDto>> GetPatientsAsync(
            string? keyword, int page, int pageSize, CancellationToken ct)
        {
            var query = _ctx.Patients.Select(p => new AccountDto
            {
                Id = p.PatientUserId,
                Role = "Patient",
                FullName = p.FullName,
                Email = p.Email,
                Phone = p.Phone,
                IsActive = p.IsActive
            });

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                    x.Email.Contains(keyword) ||
                    x.Phone.Contains(keyword) ||
                    x.FullName.Contains(keyword));
            }

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize)
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

        public async Task<AccountDto?> GetPatientByEmailAsync(
            string email, CancellationToken ct)
        {
            email = email.Trim().ToLower();

            return await _ctx.Patients
                .Where(p => p.Email.ToLower() == email)
                .Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Role = "Patient",
                    FullName = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                }).FirstOrDefaultAsync(ct);
        }

        public async Task<bool> UpdatePatientStatusAsync(
            int patientId, bool isActive, CancellationToken ct)
        {
            var patient = await _ctx.Patients.FindAsync(new object[] { patientId }, ct);
            if (patient == null) return false;

            patient.IsActive = isActive;
            patient.UpdatedAtUtc = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<int> BulkUpdatePatientStatusAsync(
            IEnumerable<int> patientIds, bool isActive, CancellationToken ct)
        {
            var patients = await _ctx.Patients
                .Where(p => patientIds.Contains(p.PatientUserId))
                .ToListAsync(ct);

            foreach (var p in patients)
            {
                p.IsActive = isActive;
                p.UpdatedAtUtc = DateTime.UtcNow;
            }

            if (patients.Count > 0) await _ctx.SaveChangesAsync(ct);
            return patients.Count;
        }

        public async Task<bool> ResetPatientPasswordAsync(
            int patientId, string newPassword, CancellationToken ct)
        {
            var patient = await _ctx.Patients.FindAsync(new object[] { patientId }, ct);
            if (patient == null) return false;

            patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            patient.UpdatedAtUtc = DateTime.UtcNow;
            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<AccountDto>> FilterPatientsByStatusAsync(
            bool isActive, CancellationToken ct)
        {
            return await _ctx.Patients
                .Where(p => p.IsActive == isActive)
                .Select(p => new AccountDto
                {
                    Id = p.PatientUserId,
                    Role = "Patient",
                    FullName = p.FullName,
                    Email = p.Email,
                    Phone = p.Phone,
                    IsActive = p.IsActive
                }).ToListAsync(ct);
        }

        public async Task<DoctorProfileDto?> GetMyProfileAsync(
            int doctorId, CancellationToken ct)
        {
            return await (from e in _ctx.Employees
                          join er in _ctx.EmployeeRoles on e.EmployeeUserId equals er.EmployeeId
                          join r in _ctx.Roles on er.RoleId equals r.RoleId
                          where e.EmployeeUserId == doctorId && r.Name == "Doctor"
                          select new DoctorProfileDto
                          {
                              DoctorId = e.EmployeeUserId,
                              FullName = e.FullName,
                              Email = e.Email,
                              Phone = e.Phone,
                              Degree = (from d in _ctx.DoctorProfiles
                                        where d.EmployeeId == e.EmployeeUserId
                                        select d.Degree).FirstOrDefault(),
                              Title = (from d in _ctx.DoctorProfiles
                                       where d.EmployeeId == e.EmployeeUserId
                                       select d.Title).FirstOrDefault(),
                              Image = e.Image,
                              IsActive = e.IsActive
                          }).FirstOrDefaultAsync(ct);
        }

        public async Task<bool> UpdateMyProfileAsync(
            int doctorId, UpdateStaffProfileRequest req, CancellationToken ct)
        {
            var doctor = await _ctx.Employees
                .Include(e => e.DoctorProfile)
                .FirstOrDefaultAsync(e => e.EmployeeUserId == doctorId, ct);

            if (doctor == null) return false;

            // Cập nhật thông tin cơ bản của Employee
            if (!string.IsNullOrEmpty(req.FullName)) doctor.FullName = req.FullName;
            if (!string.IsNullOrEmpty(req.Phone)) doctor.Phone = req.Phone;
            if (!string.IsNullOrEmpty(req.Image)) doctor.Image = req.Image;
            doctor.UpdatedAtUtc = DateTime.UtcNow;

            // Cập nhật thông tin chi tiết của DoctorProfile (không đụng đến khoa)
            if (doctor.DoctorProfile != null)
            {
                if (!string.IsNullOrEmpty(req.Degree)) doctor.DoctorProfile.Degree = req.Degree;
                if (!string.IsNullOrEmpty(req.Title)) doctor.DoctorProfile.Title = req.Title;
                if (req.ExperienceYears.HasValue) doctor.DoctorProfile.ExperienceYears = req.ExperienceYears;
                if (!string.IsNullOrEmpty(req.Education)) doctor.DoctorProfile.Education = req.Education;
                if (!string.IsNullOrEmpty(req.Certifications)) doctor.DoctorProfile.Certifications = req.Certifications;
                if (!string.IsNullOrEmpty(req.Biography)) doctor.DoctorProfile.Biography = req.Biography;
                if (!string.IsNullOrEmpty(req.Workplace)) doctor.DoctorProfile.Workplace = req.Workplace;
                if (!string.IsNullOrEmpty(req.Image)) doctor.DoctorProfile.Image = req.Image;

                doctor.DoctorProfile.UpdatedAtUtc = DateTime.UtcNow;
            }

            await _ctx.SaveChangesAsync(ct);
            return true;
        }

    }
}
