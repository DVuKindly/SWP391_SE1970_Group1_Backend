using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class RegistrationRequest : BaseEntity
    {
        public int RegistrationRequestId { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; } = default!;

        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [MaxLength(20)]
        public string Phone { get; set; } = default!;

        [MaxLength(1000)]
        public string Content { get; set; } = default!;

        public DateTime StartDate { get; set; }  

   
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; 

        public DateTime? ProcessedAt { get; set; }

    
        [MaxLength(1000)]
        public string? InternalNote { get; set; }

        // ✅ Liên kết với Staff xử lý (nếu có)
        public int? HandledById { get; set; }
        public Employee? HandledBy { get; set; }
        public bool IsProcessed { get; set; } = false;


        // ✅ Khi đặt lịch hộ: lưu AppointmentId (liên kết)
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
    }
}
