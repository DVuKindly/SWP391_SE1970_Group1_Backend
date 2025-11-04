using ClinicManagement.Application.DTOS.Request.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Booking
{
    public class RegistrationRequestDetailDto
    {
        public int RegistrationRequestId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = default!;
        public string PaymentStatus { get; set; } = default!; // 🧾 Thêm
        public bool IsProcessed { get; set; }
        public string? InternalNote { get; set; }
        public string? HandledBy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? AppointmentInfo { get; set; }
        public int? AppointmentId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
    }

}
