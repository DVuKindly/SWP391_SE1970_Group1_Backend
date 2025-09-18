using ClinicManagement.Domain.Entity.Common;
using System;

namespace ClinicManagement.Domain.Entity
{
    public class EmployeeRole : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;


        public int? AssignedById { get; set; }
        public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
