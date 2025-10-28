using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Department
{
    public class DepartmentRequestDto
    {
        [Required(ErrorMessage = "Mã khoa không được để trống.")]
        [MaxLength(20)]
        public string Code { get; set; } = default!;

        [Required(ErrorMessage = "Tên khoa không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
