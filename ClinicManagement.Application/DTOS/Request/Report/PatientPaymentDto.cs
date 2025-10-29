using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Report
{
    public class PatientPaymentDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string DoctorName { get; set; } = default!;
        public string ExamName { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal? TotalFee { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string? TransactionCode { get; set; }
        public string Status { get; set; } = default!;
    }
}
