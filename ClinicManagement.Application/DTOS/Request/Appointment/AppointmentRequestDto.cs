using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Appointment
{
    public class AppointmentRequestDto
    {
        [Required]
        public int RegistrationRequestId { get; set; } // 👈 lấy từ request thanh toán

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Note { get; set; }
    }
}
