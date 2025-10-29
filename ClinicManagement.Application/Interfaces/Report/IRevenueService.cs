using ClinicManagement.Application.DTOS.Request.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Report
{
    public interface IRevenueService
    {
    
        Task<ServiceResult<PaymentOverviewDto>> GetPaymentOverviewAsync(DateTime? from = null, DateTime? to = null);


        Task<ServiceResult<List<PaymentRecordDto>>> GetPaymentListAsync(
            bool? isPaid = null, DateTime? from = null, DateTime? to = null, string? keyword = null);

        Task<ServiceResult<List<MonthlyRevenuePointDto>>> GetRevenueByMonthAsync(int? year = null);


        Task<ServiceResult<List<YearlyRevenuePointDto>>> GetRevenueByYearRangeAsync(int fromYear, int toYear);

        Task<ServiceResult<byte[]>> ExportPaymentsToExcelAsync(
            bool? isPaid = null, DateTime? from = null, DateTime? to = null, string? keyword = null);
    }
}
