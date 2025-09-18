using ClinicManagement.Domain.Entity.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class Permission : BaseEntity
    {
        public int PermissionId { get; set; }

        [MaxLength(150)]
        public string Name { get; set; } = default!;   // e.g. "patient.read", "appointment.approve"

        [MaxLength(300)]
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
