using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class CreateDoctorRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }

    
        public string? Title { get; set; } = "Bác sĩ";
        public string? Biography { get; set; }
        public string? Degree { get; set; }
        public string? Education { get; set; }
        public int ExperienceYears { get; set; }
        public string? Certifications { get; set; }

       
        public List<DepartmentPick> Departments { get; set; } = new();

        public class DepartmentPick
        {
            public int DepartmentId { get; set; }
            public bool IsPrimary { get; set; } = false;
        }
    }
}
