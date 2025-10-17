using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class Employee : BaseEntity
    {
        [Key]
        public int EmployeeUserId { get; set; }

        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [MaxLength(200)]
        public string PasswordHash { get; set; } = default!;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }
        public bool IsActive { get; set; } = true;

        [MaxLength(150)]
        public string FullName { get; set; } = default!;

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? Image { get; set; }

        public DoctorProfile? DoctorProfile { get; set; }

        public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();

        public ICollection<DoctorDepartment> DoctorDepartments { get; set; } = new List<DoctorDepartment>();

        public ICollection<Appointment> AppointmentsAsDoctor { get; set; } = new List<Appointment>();
        public ICollection<Appointment> AppointmentsCreated { get; set; } = new List<Appointment>();
        public ICollection<Appointment> AppointmentsApproved { get; set; } = new List<Appointment>();

        // ✅ Thêm 2 dòng này để fix lỗi:
        public ICollection<DoctorWorkPattern> WorkPatterns { get; set; } = new List<DoctorWorkPattern>();
        public ICollection<DoctorLeave> Leaves { get; set; } = new List<DoctorLeave>();
    }
}
