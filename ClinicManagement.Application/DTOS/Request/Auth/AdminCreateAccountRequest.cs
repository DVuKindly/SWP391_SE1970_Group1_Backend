using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class AdminCreateAccountRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string RoleName { get; set; } = default!;

      
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? StaffId { get; set; }
    }
}
