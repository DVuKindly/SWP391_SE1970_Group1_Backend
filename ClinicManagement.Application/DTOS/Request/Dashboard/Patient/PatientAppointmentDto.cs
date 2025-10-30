using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard.Patient
{
    public class PatientAppointmentDto
    {
        public int AppointmentId { get; set; }
        public string? DoctorName { get; set; }
        public string? ExamName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Status { get; set; }
        public bool? IsPaid { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal? TotalFee { get; set; }
    }
}
