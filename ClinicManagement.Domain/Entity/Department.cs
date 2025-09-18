using ClinicManagement.Domain.Entity.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }

        [MaxLength(50)]
        public string Code { get; set; } = default!;       // Mã khoa (duy nhất)

        [MaxLength(200)]
        public string Name { get; set; } = default!;       // Tên khoa

        [MaxLength(500)]
        public string? Description { get; set; }          // Mô tả

        public bool IsActive { get; set; } = true;

        // Quan hệ: 1 khoa nhiều dịch vụ (Exam)
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        // Quan hệ: nhiều bác sĩ (Employee) thuộc khoa
        public ICollection<DoctorDepartment> DoctorDepartments { get; set; } = new List<DoctorDepartment>();
    }
}
