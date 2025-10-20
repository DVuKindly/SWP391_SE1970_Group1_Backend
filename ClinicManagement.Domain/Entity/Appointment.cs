using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;

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


        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public int DoctorId { get; set; }
        public Employee Doctor { get; set; } = default!;


        public int? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }

        public int? ApprovedById { get; set; }
        public Employee? ApprovedBy { get; set; }


        public int? ExamId { get; set; }
        public Exam? Exam { get; set; }

        public decimal? TotalFee { get; set; }

        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentAt { get; set; }


        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
