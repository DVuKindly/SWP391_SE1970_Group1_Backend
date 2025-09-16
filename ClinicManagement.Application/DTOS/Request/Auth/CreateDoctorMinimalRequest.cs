using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class CreateDoctorMinimalRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public List<DepartmentPick> Departments { get; set; } = new();

        public class DepartmentPick
        {
            public int DepartmentId { get; set; }
            public bool IsPrimary { get; set; } = false;
        }
    }
}
