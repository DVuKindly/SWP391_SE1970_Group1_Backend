using ClinicManagement.Application.DTOS.Request.Department;
using ClinicManagement.Application.DTOS.Response.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Department
{
    public interface IDepartmentService
    {
        Task<ServiceResult<List<DepartmentResponseDto>>> GetAllDepartmentsAsync(bool includeInactive = false);
        Task<ServiceResult<DepartmentResponseDto>> GetDepartmentByIdAsync(int id);
        Task<ServiceResult<DepartmentResponseDto>> CreateDepartmentAsync(DepartmentRequestDto request);
        Task<ServiceResult<DepartmentResponseDto>> UpdateDepartmentAsync(DepartmentUpdateDto request);
        Task<ServiceResult<bool>> DeleteDepartmentAsync(int id, bool softDelete = true);
    }
}
