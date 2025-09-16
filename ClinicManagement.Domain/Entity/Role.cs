using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }              
        public string Name { get; set; } = default!;
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
