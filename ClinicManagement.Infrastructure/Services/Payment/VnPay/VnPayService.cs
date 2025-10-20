using ClinicManagement.Infrastructure.Services.Payment.VNPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace ClinicManagement.Infrastructure.Services.Payment.VNPAY
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayConfig _config;

        public VnPayService(IOptions<VnPayConfig> config)
        {
            _config = config.Value;
        }

        public string CreatePaymentUrl(string txnRef, decimal amount, string clientIp)
        {
            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", _config.TmnCode);
            vnPay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnPay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", clientIp);
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toán giao dịch {txnRef}");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", _config.ReturnUrl);
            vnPay.AddRequestData("vnp_TxnRef", txnRef);

            if (!string.IsNullOrWhiteSpace(_config.IpnUrl))
                vnPay.AddRequestData("vnp_IpnUrl", _config.IpnUrl);

            return vnPay.CreateRequestUrl(_config.BaseUrl, _config.HashSecret, _config.HashEncodeUrl);
        }



        public bool ValidateResponse(IQueryCollection query)
        {
            var vnPay = new VnPayLibrary();

            foreach (var kvp in query)
            {
                var key = kvp.Key;
                var value = kvp.Value.ToString();

                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    vnPay.AddResponseData(key, value);
            }

            var vnpSecureHash = query["vnp_SecureHash"];
            return vnPay.ValidateSignature(vnpSecureHash!, _config.HashSecret, _config.HashEncodeUrl);

        }
    }
}
