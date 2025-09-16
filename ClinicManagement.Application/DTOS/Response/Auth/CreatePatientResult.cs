using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response.Auth
{
    public class CreatePatientResult
    {
        public int PatientId { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; } = default!;
    }
}
