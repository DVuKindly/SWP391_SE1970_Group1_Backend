using ClinicManagement.Application.DTOS.Response;
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
    }
}
