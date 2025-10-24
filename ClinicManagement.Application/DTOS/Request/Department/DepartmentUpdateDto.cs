using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Department
{
    public class DepartmentUpdateDto
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Tên khoa không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
