using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Prescription
{
    public class PrescriptionRequestDto
    {
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; } = default!;
        public string? Note { get; set; }


        public List<PrescriptionMedicineDto> Medicines { get; set; } = new();

    }
}
