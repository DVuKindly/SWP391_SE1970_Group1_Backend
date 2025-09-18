using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Admin : BaseEntity
    {
        public int AdminId { get; set; }

        
        public string Email { get; set; } = default!;
        public string? PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public string FullName { get; set; } = "Administrator";
        public string? Phone { get; set; }
    }
}
