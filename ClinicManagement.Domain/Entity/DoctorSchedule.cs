using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorSchedule : BaseEntity
    {
        public int ScheduleId { get; set; }

        // Bác sĩ (EmployeeId)
  
        public Employee Doctor { get; set; } = default!;

        // Khoảng thời gian làm việc
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        // Người tạo (staff hoặc admin)
        public int? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
