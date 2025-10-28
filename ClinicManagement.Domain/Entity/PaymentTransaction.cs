using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class PaymentTransaction : BaseEntity
    {
        [Key]
        public int TransactionId { get; set; }

        [MaxLength(50)]
        public string TransactionCode { get; set; } = default!;

        [MaxLength(20)]
        public string PaymentMethod { get; set; } = "VNPay";

        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime? PaymentDate { get; set; }

        //  Liên kết trực tiếp với RegistrationRequest
        public int RegistrationRequestId { get; set; }
        public RegistrationRequest RegistrationRequest { get; set; } = default!;

        //  AppointmentId có thể bỏ hoặc để nullable 
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [MaxLength(500)]
        public string? ResponseData { get; set; }
    }

}
