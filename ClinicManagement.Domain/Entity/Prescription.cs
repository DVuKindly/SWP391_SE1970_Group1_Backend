using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int? StaffId { get; set; } 
        public string Diagnosis { get; set; } = default!;
        public string? Note { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public Appointment Appointment { get; set; } = default!;
        public Employee Staff { get; set; } = default!;
        public ICollection<PrescriptionDetail> Details { get; set; } = new List<PrescriptionDetail>();
    }
}
