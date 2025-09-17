using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.DTOS.Response
{
    public class TokenResult
    {
        public string AccessToken { get; set; } = default!;
        public DateTime AccessExpiry { get; set; }
        public string RefreshToken { get; set; } = default!;
        public DateTime RefreshExpiry { get; set; }
    }
}
