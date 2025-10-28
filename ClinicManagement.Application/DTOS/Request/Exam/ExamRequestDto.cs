using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Exam
{
    public class ExamRequestDto
    {
        [Required(ErrorMessage = "Tên gói khám không được để trống.")]
        [MaxLength(200, ErrorMessage = "Tên gói khám tối đa 200 ký tự.")]
        public string ExamName { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương.")]
        public decimal Price { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
