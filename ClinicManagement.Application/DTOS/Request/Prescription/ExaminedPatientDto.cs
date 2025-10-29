using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Prescription
{
    public class ExaminedPatientDto
    {
        public int AppointmentId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string ExamName { get; set; } = default!;
        public DateTime? ExaminedAt { get; set; }
    }
}
