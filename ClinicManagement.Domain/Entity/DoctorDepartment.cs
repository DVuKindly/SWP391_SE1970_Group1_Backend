using ClinicManagement.Domain.Entity.Common;
using System;

namespace ClinicManagement.Domain.Entity
{
    /// <summary>
    /// Bác sĩ – Khoa (N-N) 
    /// Mỗi bác sĩ có thể thuộc nhiều khoa, 
    /// và mỗi khoa có nhiều bác sĩ.
    /// </summary>
    public class DoctorDepartment : BaseEntity
    {
        public int DoctorDepartmentId { get; set; }   // Khóa chính (có thể dùng composite key, nhưng để đơn giản thêm Id)

   
        public Employee Doctor { get; set; } = default!;

        public int DepartmentId { get; set; }         // Khóa ngoại -> Department
        public Department Department { get; set; } = default!;

        /// <summary>
        /// Đánh dấu đây là khoa chính của bác sĩ
        /// </summary>
        public bool IsPrimary { get; set; } = false;
    }
}
