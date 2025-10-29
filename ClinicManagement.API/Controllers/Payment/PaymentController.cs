using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Payment.VNPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("invoice/{transactionId}")]
        public async Task<IActionResult> GetInvoice(int transactionId, [FromServices] ClinicDbContext _context)
        {
            var invoice = await _context.Invoices
                .Include(i => i.RegistrationRequest)
                    .ThenInclude(r => r.Exam)
                .Include(i => i.PaymentTransaction)
                .FirstOrDefaultAsync(i => i.PaymentTransactionId == transactionId);

            if (invoice == null)
            {
                string htmlNotFound = @"
        <html><body style='font-family:Arial;text-align:center;padding-top:100px'>
            <h1 style='color:#d32f2f'>❌ Không tìm thấy hóa đơn</h1>
            <p>Hóa đơn không tồn tại hoặc chưa được tạo cho giao dịch này.</p>
            <a href='http://localhost:5173' style='color:#2A4D9B;text-decoration:none;font-weight:bold'>
                Về trang chủ
            </a>
        </body></html>";
                return Content(htmlNotFound, "text/html");
            }

            var reg = invoice.RegistrationRequest;
            var examName = reg.Exam?.Name ?? "Gói khám chưa xác định";
            var amount = invoice.TotalAmount.ToString("N0");

            string html = $@"
    <html lang='vi'>
    <head>
        <meta charset='utf-8'/>
        <title>Hóa đơn thanh toán - {invoice.InvoiceCode}</title>
        <style>
            body {{
                font-family: Arial;
                background: #f7f9ff;
                color: #333;
                line-height: 1.6;
                padding: 40px;
            }}
            .invoice {{
                background: white;
                border-radius: 10px;
                padding: 30px 50px;
                max-width: 700px;
                margin: auto;
                box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            }}
            .header {{
                text-align: center;
                border-bottom: 2px solid #2A4D9B;
                padding-bottom: 15px;
                margin-bottom: 30px;
            }}
            .header h1 {{
                color: #2A4D9B;
                margin-bottom: 5px;
            }}
            table {{
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }}
            th, td {{
                padding: 10px;
                border-bottom: 1px solid #ddd;
                text-align: left;
            }}
            th {{
                background: #eaf0ff;
            }}
            .total {{
                text-align: right;
                font-weight: bold;
                color: #2A4D9B;
            }}
            .footer {{
                margin-top: 40px;
                text-align: center;
                font-size: 14px;
                color: #555;
            }}
        </style>
    </head>
    <body>
        <div class='invoice'>
            <div class='header'>
                <h1>ClinicCare</h1>
                <p>Hóa đơn thanh toán dịch vụ</p>
            </div>

            <p><strong>Mã hóa đơn:</strong> {invoice.InvoiceCode}</p>
            <p><strong>Ngày lập:</strong> {invoice.IssuedDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Khách hàng:</strong> {reg.FullName}</p>
            <p><strong>Email:</strong> {reg.Email}</p>

            <table>
                <thead>
                    <tr>
                        <th>Dịch vụ</th>
                        <th>Đơn giá (VNĐ)</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{examName}</td>
                        <td>{amount}</td>
                    </tr>
                </tbody>
            </table>

            <p class='total'>Tổng cộng: {amount} VNĐ</p>

            <div class='footer'>
                <hr style='margin:20px 0; border:none; border-top:1px solid #ccc;'/>
                <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong>ClinicCare</strong>.</p>
                <p><em>Liên hệ hỗ trợ: support@cliniccare.vn</em></p>
            </div>
        </div>
    </body>
    </html>";

            return Content(html, "text/html");
        }



    }
}
