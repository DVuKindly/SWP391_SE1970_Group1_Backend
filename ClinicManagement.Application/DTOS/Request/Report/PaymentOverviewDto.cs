using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Report
{
    public class PaymentOverviewDto
    {
        public decimal TotalPaid { get; set; }
        public int CountPaid { get; set; }
        public decimal TotalUnpaid { get; set; }
        public int CountUnpaid { get; set; }
        public decimal GrandTotal => TotalPaid + TotalUnpaid;
        public int GrandCount => CountPaid + CountUnpaid;
    }
}
