using ClinicManagement.Domain.Entity.Common;
using System.Collections.Generic;
using System.Security;

namespace ClinicManagement.Domain.Entity
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = default!;
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }

}
