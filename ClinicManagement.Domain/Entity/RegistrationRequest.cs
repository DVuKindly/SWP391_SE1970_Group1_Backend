using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool IsProcessed { get; set; } = false;

        public DateTime StartDate { get; set; }   // Ngày khám
    }

}
