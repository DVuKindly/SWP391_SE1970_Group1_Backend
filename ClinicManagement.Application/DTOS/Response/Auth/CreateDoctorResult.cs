using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response.Auth
{
    public class CreateDoctorResult
    {
        public int DoctorId { get; set; }
        public string Email { get; set; } = default!;
  
        public List<DepartmentDto> Departments { get; set; } = new();
    }
}
