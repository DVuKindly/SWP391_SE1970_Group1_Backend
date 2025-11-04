using ClinicManagement.Domain.Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Domain.Entity
{
    public enum PaymentStatus
    {
        Unpaid,         // chưa thanh toán
        DirectPaid,     // thanh toán trực tiếp tại quầy
        VnPayPaid,      // thanh toán qua VNPay
        Refunded        // đã hoàn tiền
    }

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

        public int? HandledById { get; set; }
        public Employee? HandledBy { get; set; }

        public bool IsProcessed { get; set; } = false;

        // 🔹 Liên kết tới gói khám
        public int? ExamId { get; set; }
        public Exam? Exam { get; set; }

        // 🔹 Lưu giá khám cuối cùng 
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; }

        // 🔹 Liên kết lịch hẹn
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        // 🔹 Thêm trạng thái thanh toán
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    }
}
