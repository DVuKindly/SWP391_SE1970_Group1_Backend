using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Exam : BaseEntity
    {
        public int ExamId { get; set; }
        public string Examination { get; set; } = default!;
        public int? ExaminationType { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public bool IsActive { get; set; } = true;

        public decimal? Price { get; set; }
    }
}
