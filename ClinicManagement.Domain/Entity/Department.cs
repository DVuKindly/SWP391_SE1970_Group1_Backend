using ClinicManagement.Domain.Entity.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Domain.Entity
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }

        [MaxLength(50)]
        public string Code { get; set; } = default!;

        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<DoctorDepartment> DoctorDepartments { get; set; } = new List<DoctorDepartment>();
    }
}
