using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Patient : BaseEntity
    {
        public int PatientId { get; set; }

        // auth
        public string Email { get; set; } = default!;
        public string? PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int? RoleId { get; set; }
        public Role? Role { get; set; }

        // profile
        public string Phone { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? Note { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
