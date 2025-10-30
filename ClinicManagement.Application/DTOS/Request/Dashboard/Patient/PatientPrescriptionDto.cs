using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard.Patient
{
    public class PatientPrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public string? DoctorName { get; set; }
        public string? Diagnosis { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<PrescriptionMedicineItemDto>? Medicines { get; set; } = new();
    }
}
