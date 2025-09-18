using ClinicManagement.Domain.Entity.Common;

namespace ClinicManagement.Domain.Entity
{
    public class RolePermission : BaseEntity
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;
    }
}
