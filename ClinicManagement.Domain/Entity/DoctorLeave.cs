using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorLeave : BaseEntity
    {
        [Key]
        public int LeaveId { get; set; }

        public int DoctorId { get; set; }
        public Employee Doctor { get; set; } = default!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}
