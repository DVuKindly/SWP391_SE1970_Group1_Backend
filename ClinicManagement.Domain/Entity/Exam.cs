using ClinicManagement.Domain.Entity.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Domain.Entity
{
    public class Exam : BaseEntity
    {
        public int ExamId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; } = default!;       // Tên dịch vụ khám

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Department liên quan (có thể null nếu dịch vụ tổng quát)
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
