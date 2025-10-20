using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Response;
using ClinicManagement.Application.Interfaces.Booking;
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
    }
}
