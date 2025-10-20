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



        public async Task<bool> ProcessReturnAsync(IQueryCollection query)
        {
            bool isValid = _vnPayService.ValidateResponse(query);
            string txnRef = query["vnp_TxnRef"].ToString();

            // 1️⃣ Tìm giao dịch trong DB
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(x => x.TransactionCode == txnRef);

            if (transaction == null)
                return false;

            // 2️⃣ Lưu dữ liệu phản hồi từ VNPay
            transaction.ResponseData = string.Join("&", query.Select(q => $"{q.Key}={q.Value}"));
            transaction.PaymentDate = DateTime.Now;
            transaction.Status = isValid ? "Success" : "Failed";

            // 3️⃣ Nếu thanh toán thành công, cập nhật RegistrationRequest
            if (isValid)
            {
                // Tìm theo Appointment hoặc dựa trên PaymentTransaction gần nhất
                var reg = await _context.RegistrationRequests
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .FirstOrDefaultAsync(r =>
                        r.AppointmentId == transaction.AppointmentId ||
                        (r.Status == "Contacted" && (r.Fee == transaction.Amount || r.Exam!.Price == transaction.Amount))
                    );

                if (reg != null)
                {
                    reg.Status = "Paid";
                    reg.ProcessedAt = DateTime.Now;
                    reg.IsProcessed = true;
                    reg.Fee = reg.Exam?.Price ?? transaction.Amount;
                }
            }

            await _context.SaveChangesAsync();
            return isValid;
        }
    }
}
