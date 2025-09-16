using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Appointment : BaseEntity
    {
        public int AppointmentId { get; set; }

        public int? ScheduleId { get; set; }
        public DoctorSchedule? Schedule { get; set; }

        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = default!;

        public int? ExamId { get; set; }
        public Exam? Exam { get; set; }

        public DateTime? AppointmentDate { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }

        // pending/approved/rejected/cancelled/completed
        public string Status { get; set; } = "pending";

        public int? ApprovedByDoctorId { get; set; }
        public Doctor? ApprovedByDoctor { get; set; }
        public int? CancelledByAccountId { get; set; }
        public Account? CancelledByAccount { get; set; }
    }
}
