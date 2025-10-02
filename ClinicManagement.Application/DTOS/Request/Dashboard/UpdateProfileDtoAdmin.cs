using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Dashboard
{
    public class UpdateProfileDtoAdmin
    {
   

        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
}
