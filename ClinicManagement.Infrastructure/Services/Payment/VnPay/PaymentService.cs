using ClinicManagement.Application;
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
            string responseCode = query["vnp_ResponseCode"].ToString();

            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(x => x.TransactionCode == txnRef);

            if (transaction == null)
                return false;

            transaction.ResponseData = string.Join("&", query.Select(q => $"{q.Key}={q.Value}"));
            transaction.PaymentDate = DateTime.Now;

            if (isValid && responseCode == "00")
            {
                if (transaction.Status != "Success")
                {
                    transaction.Status = "Success";

                    var reg = await _context.RegistrationRequests
                        .Include(r => r.Exam)
                        .FirstOrDefaultAsync(r =>
                            r.AppointmentId == transaction.AppointmentId ||
                            (r.Status == "Contacted" &&
                             (r.Fee == transaction.Amount || r.Exam!.Price == transaction.Amount))
                        );

                    if (reg != null)
                    {
                        reg.PaymentStatus = PaymentStatus.VnPayPaid; 
                        reg.Status = "Scheduled"; 
                        reg.ProcessedAt = DateTime.Now;
                        reg.IsProcessed = true;
                        reg.Fee = reg.Exam?.Price ?? transaction.Amount;
                    }
                }
            }
            else
            {
                if (transaction.Status != "Success")
                    transaction.Status = "Failed";
            }

            await _context.SaveChangesAsync();
            return isValid;
        }



        public async Task<ServiceResult<Invoice>> CreateInvoiceForDirectPaymentAsync(int requestId)
        {
            var req = await _context.RegistrationRequests
                .Include(r => r.Exam)
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(r => r.RegistrationRequestId == requestId);

            if (req == null)
                return ServiceResult<Invoice>.Fail("Không tìm thấy đăng ký khám.");

            var appointment = req.Appointment;

            // 🔹 Kiểm tra trạng thái tổng quát
            if (req.Status == "Invalid" || req.Status == "Rejected")
                return ServiceResult<Invoice>.Fail("Không thể tạo phiếu thu cho đăng ký không hợp lệ hoặc bị từ chối.");

            // 🔹 Kiểm tra PaymentStatus (tránh tạo lại)
            if (req.PaymentStatus == PaymentStatus.VnPayPaid || req.PaymentStatus == PaymentStatus.DirectPaid)
                return ServiceResult<Invoice>.Fail("Đăng ký này đã được thanh toán, không thể tạo phiếu thu mới.");

            // 🔹 Validate lịch hẹn (nếu có)
            if (appointment != null)
            {
                if (appointment.IsPaid)
                    return ServiceResult<Invoice>.Fail("Lịch hẹn này đã được thanh toán. Không thể tạo thêm phiếu thu.");

                if (appointment.PaymentMethod?.Equals("VNPAY", StringComparison.OrdinalIgnoreCase) == true)
                    return ServiceResult<Invoice>.Fail("Bệnh nhân đã thanh toán qua VNPay. Không thể tạo phiếu thu trực tiếp.");
            }

            // 🔹 Xác định số tiền cần thu
            decimal total = appointment?.TotalFee ?? req.Fee ?? req.Exam?.Price ?? 0;
            if (total <= 0)
                return ServiceResult<Invoice>.Fail("Không thể tạo phiếu thu vì chưa có thông tin phí dịch vụ.");

            // 🔹 Sinh mã phiếu thu
            string code = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

            // 🔹 Tạo hóa đơn
            var invoice = new Invoice
            {
                InvoiceCode = code,
                RegistrationRequestId = req.RegistrationRequestId,
                TotalAmount = total,
                IssuedBy = "Hệ thống tự động",
                IssuedDate = DateTime.UtcNow,
                Note = appointment != null
                    ? $"Thanh toán trực tiếp cho lịch hẹn #{appointment.AppointmentId}"
                    : "Thanh toán trực tiếp tại quầy"
            };

            _context.Invoices.Add(invoice);

            // 🔹 Cập nhật trạng thái thanh toán cho lịch hẹn (nếu có)
            if (appointment != null)
            {
                appointment.IsPaid = true;
                appointment.PaymentAt = DateTime.UtcNow;
                appointment.PaymentMethod = "Direct";
                appointment.TransactionCode = code;
                appointment.Status = AppointmentStatus.Confirmed;
                _context.Appointments.Update(appointment);
            }

            // 🔹 Cập nhật PaymentStatus cho RegistrationRequest
            req.PaymentStatus = PaymentStatus.DirectPaid;
            req.ProcessedAt = DateTime.UtcNow;
            req.IsProcessed = true;
            req.UpdatedAtUtc = DateTime.UtcNow;

            // 🔹 Ghi chú nội bộ
            string prefix = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] Hệ thống: ";
            req.InternalNote = (req.InternalNote ?? "") + "\n" + prefix +
                $"Đã lập phiếu thu {invoice.InvoiceCode} ({invoice.TotalAmount:N0} VNĐ).";

            _context.RegistrationRequests.Update(req);
            await _context.SaveChangesAsync();

            // 🔹 Gửi email cho bệnh nhân (nếu có)
            if (appointment?.Patient?.Email != null)
            {
                string patientName = appointment.Patient.FullName;
                string examName = req.Exam?.Name ?? "Không xác định";
                string doctorName = appointment.Doctor?.FullName ?? "Chưa chỉ định";

                string subject = $"Hóa đơn thanh toán dịch vụ khám {examName}";
                string body = $@"
<div style='font-family:Arial;line-height:1.6'>
    <h2 style='color:#2A4D9B;'>Xin chào {patientName},</h2>
    <p>Bạn đã thanh toán thành công cho gói khám <strong>{examName}</strong>.</p>
    <p><strong>Mã phiếu thu:</strong> {code}</p>
    <p><strong>Số tiền:</strong> {total:N0} VNĐ</p>
    <p><strong>Bác sĩ phụ trách:</strong> {doctorName}</p>
    <p><strong>Thời gian khám:</strong> {appointment.StartTime:HH:mm dd/MM/yyyy}</p>
    <hr style='border:none;border-top:1px solid #ccc;margin:20px 0'/>
    <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong>ClinicCare</strong>.</p>
</div>";

                await _emailService.SendEmailAsync(appointment.Patient.Email, subject, body);
            }

            return ServiceResult<Invoice>.Ok(invoice);
        }


    }
}
