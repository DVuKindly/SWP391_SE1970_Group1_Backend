using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity.Common
{
    public class Patient : BaseEntity
    {
        public int PatientUserId { get; set; }

        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Thông tin hồ sơ bệnh nhân
        public string FullName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
