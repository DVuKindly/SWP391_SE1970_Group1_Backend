using ClinicManagement.Domain.Entity.Common;

using System;

namespace ClinicManagement.Domain.Entity
{
    public enum AppointmentStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        Completed
    }

    public class Appointment : BaseEntity
    {
        public int AppointmentId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Note { get; set; }

        // Patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        // Doctor (Employee)
        public int DoctorId { get; set; }
        public Employee Doctor { get; set; } = default!;

        // Staff tạo/duyệt
        public int? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }

        public int? ApprovedById { get; set; }
        public Employee? ApprovedBy { get; set; }

        // Dịch vụ khám (optional)
        public int? ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}
