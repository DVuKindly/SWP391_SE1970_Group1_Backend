using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class UpdateRoleRequest
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
