using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Appointment
{
    public class AppointmentRequestDto
    {
        [Required]
        public int RegistrationRequestId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [Required]
        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }

        public string? Note { get; set; }
    }
}
