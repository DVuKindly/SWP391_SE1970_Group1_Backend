using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class CreateStaffAccountRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string StaffRoleName { get; set; } = default!; // "Staff_Doctor" hoặc "Staff_Patient"
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
    }

}
