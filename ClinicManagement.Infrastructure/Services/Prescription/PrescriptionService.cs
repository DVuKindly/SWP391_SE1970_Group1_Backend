using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Prescription;
using ClinicManagement.Application.DTOS.Request.Report;
using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Application.Interfaces.Prescription;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Prescription
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly ClinicDbContext _context;
        private readonly IEmailService _email;
        private static DateTime NowVN => DateTime.UtcNow.AddHours(7);

        public PrescriptionService(ClinicDbContext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        // 🔹 1. Danh sách bệnh nhân đã khám (Examined)
        public async Task<ServiceResult<List<ExaminedPatientDto>>> GetExaminedPatientsAsync(string? keyword = null)
        {
            var query = _context.RegistrationRequests
                .Include(r => r.Exam)
                .Include(r => r.Appointment)
                .Where(r => r.Status == "Examined" && r.AppointmentId != null);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(r =>
                    r.FullName.ToLower().Contains(kw) ||
                    r.Email.ToLower().Contains(kw) ||
                    r.Phone.ToLower().Contains(kw));
            }

            var list = await query
                .OrderByDescending(r => r.ProcessedAt)
                .Select(r => new ExaminedPatientDto
                {
                    AppointmentId = r.AppointmentId ?? 0,
                    FullName = r.FullName,
                    Email = r.Email,
                    ExamName = r.Exam != null ? r.Exam.Name : "(Không rõ)",
                    ExaminedAt = r.ProcessedAt ?? NowVN
                })
                .ToListAsync();

            return ServiceResult<List<ExaminedPatientDto>>.Ok(list);
        }

        // 🔹 2. Kê đơn thuốc mới
        public async Task<ServiceResult<PrescriptionResponseDto>> CreatePrescriptionAsync(PrescriptionRequestDto req, int staffId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == req.AppointmentId);

            if (appointment == null)
                return ServiceResult<PrescriptionResponseDto>.Fail("Không tìm thấy lịch khám.");

            var reg = await _context.RegistrationRequests
                .FirstOrDefaultAsync(r => r.AppointmentId == appointment.AppointmentId);

            if (reg == null || reg.Status != "Examined")
                return ServiceResult<PrescriptionResponseDto>.Fail("Chỉ bệnh nhân đã khám mới được kê đơn.");

            var prescription = new Domain.Entity.Prescription
            {
                AppointmentId = appointment.AppointmentId,
                StaffId = staffId,
                Diagnosis = req.Diagnosis,
                Note = req.Note,
                CreatedAtUtc = NowVN
            };

            foreach (var med in req.Medicines)
            {
                prescription.Details.Add(new PrescriptionDetail
                {
                    MedicineName = med.MedicineName,
                    Dosage = med.Dosage,
                    Frequency = med.Frequency,
                    Duration = med.Duration,
                    Instruction = med.Instruction
                });
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            await SendPrescriptionEmailInternalAsync(appointment.Patient.Email, appointment.Patient.FullName, req);

            return ServiceResult<PrescriptionResponseDto>.Ok(new PrescriptionResponseDto
            {
                PrescriptionId = prescription.PrescriptionId,
                PatientName = appointment.Patient.FullName,
                Diagnosis = prescription.Diagnosis,
                Note = prescription.Note,
                CreatedAtUtc = prescription.CreatedAtUtc,
                Medicines = req.Medicines
            }, "Đã kê đơn thuốc thành công.");
        }

        // 🔹 3. Danh sách tất cả đơn thuốc
        public async Task<ServiceResult<List<PrescriptionResponseDto>>> GetAllPrescriptionsAsync(int? doctorId = null, int? patientId = null)
        {
            var query = _context.Prescriptions
                .Include(p => p.Details)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(p => p.Appointment.DoctorId == doctorId.Value);
            if (patientId.HasValue)
                query = query.Where(p => p.Appointment.PatientId == patientId.Value);

            var list = await query
                .OrderByDescending(p => p.CreatedAtUtc)
                .Select(p => new PrescriptionResponseDto
                {
                    PrescriptionId = p.PrescriptionId,
                    PatientName = p.Appointment.Patient.FullName,
                    Diagnosis = p.Diagnosis,
                    Note = p.Note,
                    CreatedAtUtc = p.CreatedAtUtc,
                    Medicines = p.Details.Select(d => new PrescriptionMedicineDto
                    {
                        MedicineName = d.MedicineName,
                        Dosage = d.Dosage,
                        Frequency = d.Frequency,
                        Duration = d.Duration,
                        Instruction = d.Instruction
                    }).ToList()
                })
                .ToListAsync();

            return ServiceResult<List<PrescriptionResponseDto>>.Ok(list);
        }

        // 🔹 4. Chi tiết đơn thuốc
        public async Task<ServiceResult<PrescriptionResponseDto>> GetPrescriptionDetailAsync(int id)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment).ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<PrescriptionResponseDto>.Fail("Không tìm thấy đơn thuốc.");

            var dto = new PrescriptionResponseDto
            {
                PrescriptionId = p.PrescriptionId,
                PatientName = p.Appointment.Patient.FullName,
                Diagnosis = p.Diagnosis,
                Note = p.Note,
                CreatedAtUtc = p.CreatedAtUtc,
                Medicines = p.Details.Select(d => new PrescriptionMedicineDto
                {
                    MedicineName = d.MedicineName,
                    Dosage = d.Dosage,
                    Frequency = d.Frequency,
                    Duration = d.Duration,
                    Instruction = d.Instruction
                }).ToList()
            };

            return ServiceResult<PrescriptionResponseDto>.Ok(dto);
        }

        // 🔹 5. Cập nhật đơn thuốc
        public async Task<ServiceResult<PrescriptionResponseDto>> UpdatePrescriptionAsync(int id, PrescriptionRequestDto req, int staffId)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment).ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<PrescriptionResponseDto>.Fail("Không tìm thấy đơn thuốc.");

            p.Diagnosis = req.Diagnosis;
            p.Note = req.Note;
            p.UpdatedAtUtc = NowVN;

            _context.PrescriptionDetails.RemoveRange(p.Details);
            p.Details.Clear();

            foreach (var med in req.Medicines)
            {
                p.Details.Add(new PrescriptionDetail
                {
                    MedicineName = med.MedicineName,
                    Dosage = med.Dosage,
                    Frequency = med.Frequency,
                    Duration = med.Duration,
                    Instruction = med.Instruction
                });
            }

            await _context.SaveChangesAsync();

            return ServiceResult<PrescriptionResponseDto>.Ok(new PrescriptionResponseDto
            {
                PrescriptionId = p.PrescriptionId,
                PatientName = p.Appointment.Patient.FullName,
                Diagnosis = p.Diagnosis,
                Note = p.Note,
                CreatedAtUtc = p.CreatedAtUtc,
                Medicines = req.Medicines
            }, "Cập nhật đơn thuốc thành công.");
        }

        // 🔹 6. Xoá đơn thuốc
        public async Task<ServiceResult<bool>> DeletePrescriptionAsync(int id)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<bool>.Fail("Không tìm thấy đơn thuốc.");

            _context.PrescriptionDetails.RemoveRange(p.Details);
            _context.Prescriptions.Remove(p);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Đã xoá đơn thuốc.");
        }

        // 🔹 7. Gửi lại email đơn thuốc cho bệnh nhân
        public async Task<ServiceResult<string>> SendPrescriptionEmailAsync(int id)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment).ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<string>.Fail("Không tìm thấy đơn thuốc.");

            var req = new PrescriptionRequestDto
            {
                Diagnosis = p.Diagnosis,
                Note = p.Note,
                Medicines = p.Details.Select(d => new PrescriptionMedicineDto
                {
                    MedicineName = d.MedicineName,
                    Dosage = d.Dosage,
                    Frequency = d.Frequency,
                    Duration = d.Duration,
                    Instruction = d.Instruction
                }).ToList()
            };

            await SendPrescriptionEmailInternalAsync(p.Appointment.Patient.Email, p.Appointment.Patient.FullName, req);

            return ServiceResult<string>.Ok("Đã gửi lại email đơn thuốc cho bệnh nhân.");
        }

        // 🔸 Helper gửi email
        private async Task SendPrescriptionEmailInternalAsync(string? email, string fullName, PrescriptionRequestDto req)
        {
            if (string.IsNullOrEmpty(email)) return;

            string meds = string.Join("<br/>", req.Medicines.Select(m =>
                $"• <b>{m.MedicineName}</b> - {m.Dosage} - {m.Frequency} - {m.Duration}"));

            string body = $@"
                <div style='font-family:Arial;line-height:1.6'>
                    <h2 style='color:#2A4D9B'>Đơn thuốc sau khám</h2>
                    <p>Xin chào <strong>{fullName}</strong>,</p>
                    <p><b>Chẩn đoán:</b> {req.Diagnosis}</p>
                    <p><b>Thuốc kê:</b><br/>{meds}</p>
                    <p><i>{req.Note}</i></p>
                    <hr/>
                    <p>Cảm ơn bạn đã tin tưởng <strong>ClinicCare</strong>.</p>
                </div>";

            await _email.SendEmailAsync(email, "Đơn thuốc sau khám", body);
        }

        public Task<ServiceResult<PatientPaymentSummaryDto>> GetPaymentSummaryAsync(DateTime? from = null, DateTime? to = null)
        {
            throw new NotImplementedException();
        }
    }
}
