using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services.Payment.VNPAY
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(string txnRef, decimal amount, string clientIp);
        bool ValidateResponse(IQueryCollection query);
    }
}
