using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response.Auth
{
    public class RegistrationRequestResponseDto
    {
        public int RegistrationRequestId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime StartDate { get; set; }

        public string Status { get; set; } = default!;
        public bool IsProcessed { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string? HandledBy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? InternalNote { get; set; }
    }
}
