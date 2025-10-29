using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Report
{
    public class YearlyRevenuePointDto
    {
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int AppointmentCount { get; set; }
    }
}
