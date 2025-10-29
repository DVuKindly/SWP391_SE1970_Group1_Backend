using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Report
{
    public class PatientPaymentSummaryDto
    {
        public decimal TotalPaid { get; set; }
        public decimal TotalUnpaid { get; set; }
        public int PaidCount { get; set; }
        public int UnpaidCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
