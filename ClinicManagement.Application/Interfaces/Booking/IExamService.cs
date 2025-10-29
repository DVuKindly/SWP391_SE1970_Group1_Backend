using ClinicManagement.Application.DTOS.Request.Exam;
using ClinicManagement.Application.DTOS.Response;
using ClinicManagement.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Booking
{
    public interface IExamService
    {
        Task<ServiceResult<List<ExamResponseDto>>> GetAllExamsAsync(bool includeInactive = false);
        Task<ServiceResult<ExamResponseDto>> GetExamDetailAsync(int examId);
        Task<ServiceResult<ExamResponseDto>> UpdateExamAsync(int examId, ExamRequestDto request);


        // ✅ Thêm mới
        Task<ServiceResult<ExamResponseDto>> CreateExamAsync(ExamRequestDto request);
        Task<ServiceResult<bool>> DeleteExamAsync(int examId, bool softDelete = true);

    }
}
