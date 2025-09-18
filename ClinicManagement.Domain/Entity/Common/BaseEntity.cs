using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity.Common
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        // Optimistic concurrency (tùy chọn)
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
