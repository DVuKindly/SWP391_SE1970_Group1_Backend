using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Department;
using ClinicManagement.Application.DTOS.Response.Exam;
using ClinicManagement.Application.Interfaces.Department;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ClinicDbContext _context;

        public DepartmentService(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<List<DepartmentResponseDto>>> GetAllDepartmentsAsync(bool includeInactive = false)
        {
            var query = _context.Departments.AsQueryable();

            if (!includeInactive)
                query = query.Where(d => d.IsActive);

            var list = await query
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentResponseDto
                {
                    DepartmentId = d.DepartmentId,
                    Code = d.Code,
                    Name = d.Name,
                    Description = d.Description,
                    IsActive = d.IsActive
                })
                .ToListAsync();

            return ServiceResult<List<DepartmentResponseDto>>.Ok(list);
        }

        public async Task<ServiceResult<DepartmentResponseDto>> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .Where(d => d.DepartmentId == id)
                .Select(d => new DepartmentResponseDto
                {
                    DepartmentId = d.DepartmentId,
                    Code = d.Code,
                    Name = d.Name,
                    Description = d.Description,
                    IsActive = d.IsActive
                })
                .FirstOrDefaultAsync();

            if (department == null)
                return ServiceResult<DepartmentResponseDto>.Fail("Không tìm thấy khoa.");

            return ServiceResult<DepartmentResponseDto>.Ok(department);
        }

        public async Task<ServiceResult<DepartmentResponseDto>> CreateDepartmentAsync(DepartmentRequestDto request)
        {
            bool codeExists = await _context.Departments.AnyAsync(d => d.Code.ToLower() == request.Code.ToLower());
            if (codeExists)
                return ServiceResult<DepartmentResponseDto>.Fail("Mã khoa đã tồn tại.");

            var department = new Domain.Entity.Department
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var response = new DepartmentResponseDto
            {
                DepartmentId = department.DepartmentId,
                Code = department.Code,
                Name = department.Name,
                Description = department.Description,
                IsActive = department.IsActive
            };

            return ServiceResult<DepartmentResponseDto>.Ok(response, "Tạo khoa thành công.");
        }

        public async Task<ServiceResult<DepartmentResponseDto>> UpdateDepartmentAsync(DepartmentUpdateDto request)
        {
            var department = await _context.Departments.FindAsync(request.DepartmentId);
            if (department == null)
                return ServiceResult<DepartmentResponseDto>.Fail("Không tìm thấy khoa.");

            department.Name = request.Name;
            department.Description = request.Description;
            department.IsActive = request.IsActive;
            department.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new DepartmentResponseDto
            {
                DepartmentId = department.DepartmentId,
                Code = department.Code,
                Name = department.Name,
                Description = department.Description,
                IsActive = department.IsActive
            };

            return ServiceResult<DepartmentResponseDto>.Ok(response, "Cập nhật khoa thành công.");
        }

        public async Task<ServiceResult<bool>> DeleteDepartmentAsync(int id, bool softDelete = true)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return ServiceResult<bool>.Fail("Không tìm thấy khoa.");

            if (softDelete)
            {
                department.IsActive = false;
                department.UpdatedAtUtc = DateTime.UtcNow;
            }
            else
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, softDelete ? "Đã vô hiệu hóa khoa." : "Đã xóa khoa.");
        }
    }
}
