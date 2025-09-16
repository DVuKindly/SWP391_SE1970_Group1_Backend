using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorDepartment : BaseEntity
    {
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = default!;

        public bool IsPrimary { get; set; } = false;
    }
}
