using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<DoctorDepartment> DoctorDepartments { get; set; } = new List<DoctorDepartment>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}
