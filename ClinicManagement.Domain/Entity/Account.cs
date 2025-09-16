using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Account : BaseEntity
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;

        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? StaffId { get; set; }
        public Staff? Staff { get; set; }

        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
