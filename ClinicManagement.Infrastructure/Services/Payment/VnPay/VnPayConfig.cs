using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Payment.VNPAY
{
    public class VnPayConfig
    {

        public string BaseUrl { get; set; } = default!; // ✅ Đổi từ Url -> BaseUrl

  
        public string TmnCode { get; set; } = default!;

    
        public string HashSecret { get; set; } = default!;

        public string ReturnUrl { get; set; } = default!;

        public string? IpnUrl { get; set; }

     
        public bool HashEncodeUrl { get; set; } = true;
    }
}
