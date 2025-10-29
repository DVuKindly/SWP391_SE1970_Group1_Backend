using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Payment.VNPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Payment.VNPAY
{
    public class PaymentService
    {
        private readonly ClinicDbContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailService _emailService;
        public PaymentService(ClinicDbContext context, IVnPayService vnPayService , IEmailService emailService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _emailService = emailService;
        }


        public async Task<string> CreatePaymentForRegistrationAsync(int registrationId, int examId, string clientIp)
        {
            var reg = await _context.RegistrationRequests
                .Include(r => r.Exam)
                .FirstOrDefaultAsync(r => r.RegistrationRequestId == registrationId);

            if (reg == null)
                throw new Exception("Không tìm thấy yêu cầu đăng ký khám.");

            if (reg.Status != "Contacted")
                throw new Exception("Chỉ tạo thanh toán khi yêu cầu đã ở trạng thái 'Contacted'.");

            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.ExamId == examId && e.IsActive);
            if (exam == null)
                throw new Exception("Không tìm thấy gói khám hợp lệ.");

            reg.Exam = exam;
            var amount = exam.Price;
            if (amount <= 0)
                throw new Exception("Gói khám chưa có giá hợp lệ.");

            var txnRef = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            var transaction = new PaymentTransaction
            {
                RegistrationRequestId = registrationId,
                Amount = amount,
                TransactionCode = txnRef,
                Status = "Pending",
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            // ✅ Tạo URL VNPay
            var paymentUrl = _vnPayService.CreatePaymentUrl(transaction.TransactionCode, amount, clientIp);

         
            if (!string.IsNullOrEmpty(reg.Email))
            {
                string subject = $"Thanh toán dịch vụ khám {exam.Name} tại ClinicCare";
                string body = $@"
            <div style='font-family:Arial; line-height:1.6'>
                <h2 style='color:#2A4D9B;'>Xin chào {reg.FullName},</h2>
                <p>Bạn đã đăng ký gói khám <strong>{exam.Name}</strong> với chi phí <strong>{amount:N0} VNĐ</strong>.</p>
                <p>Vui lòng bấm vào liên kết dưới đây để thanh toán an toàn qua VNPay:</p>
                <p style='margin:20px 0'>
                    <a href='{paymentUrl}' 
                       style='background:#2A4D9B; color:white; padding:12px 24px; 
                              border-radius:6px; text-decoration:none;'>Thanh toán ngay</a>
                </p>
                <p>Sau khi thanh toán thành công, nhân viên của chúng tôi sẽ liên hệ để xác nhận lịch hẹn.</p>
                <hr style='border:none;border-top:1px solid #ccc;margin:30px 0;'/>
                <small>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email.</small>
            </div>";

                await _emailService.SendEmailAsync(reg.Email, subject, body);
            }

            return paymentUrl;
        }

        public static DateTime NowVN => DateTime.UtcNow.AddHours(7);


        public async Task<bool> ProcessReturnAsync(IQueryCollection query)
        {


            bool isValid = _vnPayService.ValidateResponse(query);
            string txnRef = query["vnp_TxnRef"].ToString();
            string responseCode = query["vnp_ResponseCode"].ToString();

          
            var transaction = await _context.PaymentTransactions
                .Include(t => t.RegistrationRequest)
                .ThenInclude(r => r.Exam)
                .FirstOrDefaultAsync(x => x.TransactionCode == txnRef);

            if (transaction == null)
                return false;

     
            transaction.ResponseData = string.Join("&", query.Select(q => $"{q.Key}={q.Value}"));
            transaction.PaymentDate = DateTime.UtcNow;
            transaction.Status = isValid ? "Success" : "Failed";

        
            if (isValid)
            {
                var reg = transaction.RegistrationRequest;

                if (reg == null)
                {
              
                    reg = await _context.RegistrationRequests
                        .Include(r => r.Exam)
                        .OrderByDescending(r => r.CreatedAtUtc)
                        .FirstOrDefaultAsync(r =>
                            r.AppointmentId == transaction.AppointmentId ||
                            (r.Status == "Contacted" &&
                             (r.Fee == transaction.Amount || r.Exam!.Price == transaction.Amount))
                        );
                }


                if (reg != null)
                {
                    transaction.PaymentDate = NowVN;


                    reg.Status = "Paid";
                    reg.IsProcessed = true;
                    reg.ProcessedAt = NowVN;
                    reg.Fee = reg.Exam?.Price ?? transaction.Amount;
                    reg.UpdatedAtUtc = NowVN;

               
                    var invoice = new Invoice
                    {
                        InvoiceCode = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{reg.RegistrationRequestId}",
                        RegistrationRequestId = reg.RegistrationRequestId,
                        PaymentTransactionId = transaction.TransactionId,
                        TotalAmount = transaction.Amount,
                        IssuedDate = NowVN,
                        IssuedBy = "System Auto",
                        Note = $"Thanh toán VNPay thành công - Mã GD: {transaction.TransactionCode}"
                    };

                    _context.Invoices.Add(invoice);

         
                    if (!string.IsNullOrEmpty(reg.Email))
                    {
                        string subject = $"Hóa đơn thanh toán - {reg.FullName}";
                        string body = $@"
                <div style='font-family:Arial; line-height:1.6'>
                    <h2 style='color:#2A4D9B;'>ClinicCare Invoice</h2>
                    <p>Xin chào <strong>{reg.FullName}</strong>,</p>
                    <p>Bạn đã thanh toán thành công gói khám 
                       <strong>{reg.Exam?.Name}</strong> với số tiền 
                       <strong>{transaction.Amount:N0} VNĐ</strong>.</p>
                    <p><strong>Mã hóa đơn:</strong> {invoice.InvoiceCode}</p>
                    <p><strong>Ngày lập:</strong> {invoice.IssuedDate:dd/MM/yyyy HH:mm}</p>
                    <p><strong>Mã giao dịch:</strong> {transaction.TransactionCode}</p>
                    <hr style='border:none;border-top:1px solid #ccc;margin:20px 0'/>
                    <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi.</p>
                    <p>Trân trọng,<br/>Đội ngũ ClinicCare</p>
                </div>";

                        await _emailService.SendEmailAsync(reg.Email, subject, body);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return isValid;
        }

    }
}
