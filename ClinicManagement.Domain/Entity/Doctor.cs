using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Doctor : BaseEntity
    {
        public int DoctorId { get; set; }
        public string Phone { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Specialization { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
