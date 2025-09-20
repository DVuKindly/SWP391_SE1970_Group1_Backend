using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard.Doctor
{
    public class DoctorProfileDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Degree { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
    }
}
