using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class DoctorSchedule : BaseEntity
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = default!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxPatients { get; set; } = 1;
        public int? CreatedByStaffId { get; set; }
        public Staff? CreatedByStaff { get; set; }
    }
}
