using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Booking
{
    public class DoctorWorkPatternDto
    {
        public int WorkPatternId { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string DayName => DayOfWeek.ToString(); // tiện cho UI
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsWorking { get; set; }
        public string? Note { get; set; }
    }
}
