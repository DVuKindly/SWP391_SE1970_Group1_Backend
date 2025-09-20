using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard.Staff
{
    public class StaffProfileDto
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateStaffProfileRequest
    {
        // Thuộc Employees
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }

        // Thuộc DoctorProfile
        public string? Degree { get; set; }
        public string? Title { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Biography { get; set; }
        public string? Workplace { get; set; }
    }
}
