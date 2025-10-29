using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Prescription
{
    public class PrescriptionMedicineDto
    {
        public string MedicineName { get; set; } = default!;
        public string Dosage { get; set; } = default!;      // ví dụ: "500mg"
        public string Frequency { get; set; } = default!;   // ví dụ: "2 lần/ngày"
        public string Duration { get; set; } = default!;    // ví dụ: "5 ngày"
        public string? Instruction { get; set; }            // ví dụ: "Uống sau ăn"
    }
}
