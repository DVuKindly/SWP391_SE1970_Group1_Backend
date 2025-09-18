using ClinicManagement.Domain.Entity.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }   // Admin, Staff_Patient, Staff_Doctor, Doctor

        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(300)]
        public string? Description { get; set; }

        public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
