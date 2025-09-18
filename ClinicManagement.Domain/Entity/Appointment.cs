using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

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

        // thời gian hẹn
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // trạng thái
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Note { get; set; }

        // ====== Foreign Keys ======
        // bệnh nhân
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        // bác sĩ phụ trách
        public int DoctorId { get; set; }      // EmployeeId của bác sĩ
        public Employee Doctor { get; set; } = default!;

        // nhân viên tạo/duyệt (có thể null)
        public int? CreatedById { get; set; }  // EmployeeId
        public Employee? CreatedBy { get; set; }

        public int? ApprovedById { get; set; } // EmployeeId
        public Employee? ApprovedBy { get; set; }

    
        public int? ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}
