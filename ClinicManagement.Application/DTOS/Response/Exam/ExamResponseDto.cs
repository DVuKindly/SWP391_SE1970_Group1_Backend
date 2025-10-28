using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response.Exam
{
    public class ExamResponseDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }

}
