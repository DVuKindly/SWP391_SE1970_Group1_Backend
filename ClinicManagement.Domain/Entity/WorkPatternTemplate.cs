using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class WorkPatternTemplate : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Use DayOfWeek enum for clarity
        public DayOfWeek DayOfWeek { get; set; }

        // TimeSpan to represent time-of-day
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Slot duration in minutes (e.g., 60 for 1 hour)
        public int SlotMinutes { get; set; } = 60;

        public bool IsActive { get; set; } = true;
    }
}
