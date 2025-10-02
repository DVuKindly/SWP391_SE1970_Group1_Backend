using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Request.Auth
{
    public class GoogleLoginDto
    {
        public string IdToken { get; set; } = default!;
    }
}
