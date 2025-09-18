using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class Patient : BaseEntity
    {
        [Key]
        public int PatientUserId { get; set; }

        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [MaxLength(200)]
        public string PasswordHash { get; set; } = default!;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }
        public bool IsActive { get; set; } = true;

        // Profile
        [MaxLength(150)]
        public string FullName { get; set; } = default!;

        [MaxLength(30)]
        public string Phone { get; set; } = default!;

        [MaxLength(250)]
        public string? Address { get; set; }

        public DateTime? DOB { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
