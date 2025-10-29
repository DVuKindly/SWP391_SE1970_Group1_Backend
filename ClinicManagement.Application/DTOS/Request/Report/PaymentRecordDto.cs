using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Report
{
    public class PaymentRecordDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = default!;
        public string PatientEmail { get; set; } = default!;
        public string DoctorName { get; set; } = default!;
        public string? ExamName { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
    }
}
