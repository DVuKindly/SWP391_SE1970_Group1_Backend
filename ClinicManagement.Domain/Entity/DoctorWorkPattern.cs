using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorWorkPattern : BaseEntity
    {
        [Key]
        public int WorkPatternId { get; set; }

        public int DoctorId { get; set; }
        public Employee Doctor { get; set; } = default!;

        public DayOfWeek DayOfWeek { get; set; } // Monday..Sunday

        // Time-of-day fields (TimeSpan)
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Duration of each slot in minutes (e.g., 60)
        public int SlotMinutes { get; set; } = 60;

        public bool IsWorking { get; set; } = true;

        [MaxLength(200)]
        public string? Note { get; set; }
    }
}
