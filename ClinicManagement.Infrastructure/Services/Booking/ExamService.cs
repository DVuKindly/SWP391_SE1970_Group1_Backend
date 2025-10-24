using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Exam;
using ClinicManagement.Application.DTOS.Response;
using ClinicManagement.Application.Interfaces.Booking;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Booking
{
    public class ExamService : IExamService
    {
        private readonly ClinicDbContext _context;

        public ExamService(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<List<ExamResponseDto>>> GetAllExamsAsync(bool includeInactive = false)
        {
            var query = _context.Exams.AsQueryable();

            if (!includeInactive)
                query = query.Where(e => e.IsActive);

            var list = await query
                .OrderBy(e => e.Name)
                .Select(e => new ExamResponseDto
                {
                    ExamId = e.ExamId,
                    ExamName = e.Name,
                    Description = e.Description,
                    Price = e.Price,
                    IsActive = e.IsActive
                })
                .ToListAsync();

            return ServiceResult<List<ExamResponseDto>>.Ok(list);
        }

        public async Task<ServiceResult<ExamResponseDto>> GetExamDetailAsync(int examId)
        {
            var exam = await _context.Exams
                .Where(e => e.ExamId == examId)
                .Select(e => new ExamResponseDto
                {
                    ExamId = e.ExamId,
                    ExamName = e.Name,
                    Description = e.Description,
                    Price = e.Price,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync();

            if (exam == null)
                return ServiceResult<ExamResponseDto>.Fail("Không tìm thấy gói khám.");

            return ServiceResult<ExamResponseDto>.Ok(exam);
        }

        public async Task<ServiceResult<ExamResponseDto>> CreateExamAsync(ExamRequestDto request)
        {
            // Check trùng tên
            bool exists = await _context.Exams.AnyAsync(e => e.Name.ToLower() == request.ExamName.ToLower());
            if (exists)
                return ServiceResult<ExamResponseDto>.Fail("Tên gói khám đã tồn tại.");

            var exam = new Exam
            {
                Name = request.ExamName,
                Description = request.Description,
                Price = request.Price,
                DepartmentId = request.DepartmentId,
                IsActive = request.IsActive,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            var response = new ExamResponseDto
            {
                ExamId = exam.ExamId,
                ExamName = exam.Name,
                Description = exam.Description,
                Price = exam.Price,
                IsActive = exam.IsActive,
               
            };

            return ServiceResult<ExamResponseDto>.Ok(response, "Tạo gói khám thành công.");
        }

        public async Task<ServiceResult<bool>> DeleteExamAsync(int examId, bool softDelete = true)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null)
                return ServiceResult<bool>.Fail("Không tìm thấy gói khám cần xóa.");

            if (softDelete)
            {
                exam.IsActive = false;
                exam.UpdatedAtUtc = DateTime.UtcNow;
            }
            else
            {
                _context.Exams.Remove(exam);
            }

            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, softDelete ? "Đã vô hiệu hóa gói khám." : "Đã xóa gói khám.");
        }
    }
}
