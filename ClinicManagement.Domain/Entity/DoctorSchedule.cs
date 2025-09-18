using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorSchedule : BaseEntity
    {
        [Key]  
        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }
        public Employee Doctor { get; set; } = default!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public int? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
