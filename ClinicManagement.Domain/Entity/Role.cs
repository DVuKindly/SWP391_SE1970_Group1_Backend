using ClinicManagement.Domain.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Domain.Entity
{
    public class Role : BaseEntity
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = default!;

     
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
        public ICollection<Staff> Staffs { get; set; } = new List<Staff>();
    }
}
