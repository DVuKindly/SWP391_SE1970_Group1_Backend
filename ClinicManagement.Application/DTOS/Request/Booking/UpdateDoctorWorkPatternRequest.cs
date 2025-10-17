using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Booking
{
    public class UpdateDoctorWorkPatternRequest
    {
        [Required]
        public int WorkPatternId { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsWorking { get; set; } = true;

        [MaxLength(200)]
        public string? Note { get; set; }
    }
}
