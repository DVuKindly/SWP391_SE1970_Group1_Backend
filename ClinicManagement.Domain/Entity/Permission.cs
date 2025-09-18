using ClinicManagement.Domain.Entity.Common;
using System.Collections.Generic;

namespace ClinicManagement.Domain.Entity
{
    public class Permission : BaseEntity
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = default!; // e.g. "patient.read"
        public string? Description { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
