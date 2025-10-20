using ClinicManagement.Infrastructure.Services.Payment.VNPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.API.Controllers.Payment
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost("createpayment-for-registration")]
        public async Task<IActionResult> CreateForRegistration([FromQuery] int registrationId, [FromQuery] int examId)
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            try
            {
                var paymentUrl = await _paymentService.CreatePaymentForRegistrationAsync(registrationId, examId, clientIp);

                return Ok(new
                {
                    success = true,
                    registrationId,
                    examId,
                    paymentUrl,
                    message = "Tạo link thanh toán thành công. Gửi URL này cho bệnh nhân để thực hiện thanh toán."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet("vnpayreturn")]
        public async Task<IActionResult> VnPayReturn()
        {
            try
            {
                bool success = await _paymentService.ProcessReturnAsync(Request.Query);

                if (success)
                {
                    string html = @"
                <html lang='vi'>
                <head>
                    <meta charset='utf-8'/>
                    <title>Thanh toán thành công</title>
                    <style>
                        body { font-family: Arial; background: #f5f8ff; text-align:center; padding-top:100px; }
                        .card {
                            display:inline-block;
                            background:white;
                            border-radius:10px;
                            padding:40px 60px;
                            box-shadow:0 4px 15px rgba(0,0,0,0.1);
                        }
                        h1 { color:#2A4D9B; margin-bottom:15px; }
                        p { color:#444; font-size:16px; }
                        .btn {
                            display:inline-block;
                            margin-top:25px;
                            padding:12px 25px;
                            background:#2A4D9B;
                            color:white;
                            text-decoration:none;
                            border-radius:6px;
                            transition:0.3s;
                        }
                        .btn:hover { background:#203b7a; }
                    </style>
                </head>
                <body>
                    <div class='card'>
                        <h1>🎉 Thanh toán thành công!</h1>
                        <p>Cảm ơn bạn đã sử dụng dịch vụ của <strong>Clinic</strong>.</p>
                        <p>Chúng tôi sẽ sớm liên hệ để xác nhận lịch hẹn khám.</p>
                        <a href='http://localhost:5173' class='btn'>Về trang chủ</a>
                    </div>
                </body>
                </html>";
                    return Content(html, "text/html");
                }
                else
                {
                    string html = @"
                <html lang='vi'>
                <head>
                    <meta charset='utf-8'/>
                    <title>Thanh toán thất bại</title>
                    <style>
                        body { font-family: Arial; background: #fff3f3; text-align:center; padding-top:100px; }
                        .card {
                            display:inline-block;
                            background:white;
                            border-radius:10px;
                            padding:40px 60px;
                            box-shadow:0 4px 15px rgba(0,0,0,0.1);
                        }
                        h1 { color:#d32f2f; margin-bottom:15px; }
                        p { color:#444; font-size:16px; }
                        .btn {
                            display:inline-block;
                            margin-top:25px;
                            padding:12px 25px;
                            background:#d32f2f;
                            color:white;
                            text-decoration:none;
                            border-radius:6px;
                            transition:0.3s;
                        }
                        .btn:hover { background:#a62828; }
                    </style>
                </head>
                <body>
                    <div class='card'>
                        <h1>❌ Thanh toán thất bại!</h1>
                        <p>Đã có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.</p>
                        <a href='http://localhost:5173' class='btn'>Quay lại</a>
                    </div>
                </body>
                </html>";
                    return Content(html, "text/html");
                }
            }
            catch (Exception ex)
            {
                string html = $@"
            <html><body style='font-family:Arial;text-align:center;padding-top:100px'>
                <h1 style='color:red'>⚠️ Lỗi xử lý thanh toán!</h1>
                <p>{ex.Message}</p>
                <a href='http://localhost:5173' style='color:#2A4D9B;text-decoration:none;font-weight:bold'>Về trang chủ</a>
            </body></html>";
                return Content(html, "text/html");
            }
        }

    }
}
