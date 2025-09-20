using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
