using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Invoice : BaseEntity
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required, MaxLength(20)]
        public string InvoiceCode { get; set; } = default!;

        public int RegistrationRequestId { get; set; }
        public RegistrationRequest RegistrationRequest { get; set; } = default!;

        public int? PaymentTransactionId { get; set; }
        public PaymentTransaction? PaymentTransaction { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? IssuedBy { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }

        [MaxLength(255)]
        public string? FileUrl { get; set; } 
    }
}
