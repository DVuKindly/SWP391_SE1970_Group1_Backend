using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Report
{
    public class MonthlyRevenuePointDto
    {
        public int Month { get; set; }     // 1..12
        public decimal Revenue { get; set; }
        public int AppointmentCount { get; set; }
    }
}
