using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response.Appoitment
{
    public class EligiblePatientResponseDto
    {
        public int RegistrationRequestId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? ExamName { get; set; }
        public decimal? Fee { get; set; }
    }
}
