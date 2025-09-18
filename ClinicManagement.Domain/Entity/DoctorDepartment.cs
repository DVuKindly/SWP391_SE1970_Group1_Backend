using ClinicManagement.Domain.Entity.Common;


namespace ClinicManagement.Domain.Entity
{
    public class DoctorDepartment : BaseEntity
    {
        public int DoctorDepartmentId { get; set; }

        public int DoctorId { get; set; }
        public Employee Doctor { get; set; } = default!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = default!;

        public bool IsPrimary { get; set; } = false;
    }
}
