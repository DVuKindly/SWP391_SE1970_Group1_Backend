using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response.Appoitment
{
    public class DoctorScheduleResponseDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = default!;
        public string? DepartmentName { get; set; }
        public string? Title { get; set; }
        public string? Degree { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }

        public List<WorkPatternDto> WorkPatterns { get; set; } = new();
    }

    public class WorkPatternDto
    {
        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = default!;
        public string StartTime { get; set; } = default!;
        public string EndTime { get; set; } = default!;
        public bool IsWorking { get; set; }
    }
}
