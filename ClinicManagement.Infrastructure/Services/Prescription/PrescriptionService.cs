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

        // ✅ 1. Lấy danh sách đơn thuốc thuộc các bệnh nhân của bác sĩ
        public async Task<ServiceResult<List<PrescriptionResponseDto>>> GetAllPrescriptionsForDoctorAsync(int doctorId)
        {
            var list = await _context.Prescriptions
                .Include(p => p.Details)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .Where(p => p.Appointment.DoctorId == doctorId)
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

        // ✅ 2. Xem chi tiết đơn thuốc của bệnh nhân thuộc bác sĩ
        public async Task<ServiceResult<PrescriptionResponseDto>> GetPrescriptionDetailForDoctorAsync(int id, int doctorId)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment).ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<PrescriptionResponseDto>.Fail("Không tìm thấy đơn thuốc.");

            if (p.Appointment.DoctorId != doctorId)
                return ServiceResult<PrescriptionResponseDto>.Fail("Bạn không có quyền xem đơn thuốc này.");

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

        // ✅ 3. Kê đơn thuốc mới (chỉ cho bệnh nhân của mình)
        public async Task<ServiceResult<PrescriptionResponseDto>> CreatePrescriptionAsync(PrescriptionRequestDto req, int doctorId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == req.AppointmentId);

            if (appointment == null)
                return ServiceResult<PrescriptionResponseDto>.Fail("Không tìm thấy lịch khám.");

            if (appointment.DoctorId != doctorId)
                return ServiceResult<PrescriptionResponseDto>.Fail("Bạn không có quyền kê đơn cho bệnh nhân này.");

            var reg = await _context.RegistrationRequests
                .FirstOrDefaultAsync(r => r.AppointmentId == appointment.AppointmentId);

            if (reg == null || reg.Status != "Examined")
                return ServiceResult<PrescriptionResponseDto>.Fail("Chỉ bệnh nhân đã khám mới được kê đơn.");

            var prescription = new Domain.Entity.Prescription
            {
                AppointmentId = appointment.AppointmentId,
                StaffId = doctorId,
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

        // ✅ 4. Cập nhật đơn thuốc (chỉ đơn của bác sĩ)
        public async Task<ServiceResult<PrescriptionResponseDto>> UpdatePrescriptionForDoctorAsync(int id, PrescriptionRequestDto req, int doctorId)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment).ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<PrescriptionResponseDto>.Fail("Không tìm thấy đơn thuốc.");

            if (p.Appointment.DoctorId != doctorId)
                return ServiceResult<PrescriptionResponseDto>.Fail("Bạn không có quyền sửa đơn thuốc này.");

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

        // ✅ 5. Xoá đơn thuốc
        public async Task<ServiceResult<bool>> DeletePrescriptionForDoctorAsync(int id, int doctorId)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<bool>.Fail("Không tìm thấy đơn thuốc.");

            if (p.Appointment.DoctorId != doctorId)
                return ServiceResult<bool>.Fail("Bạn không có quyền xoá đơn thuốc này.");

            _context.PrescriptionDetails.RemoveRange(p.Details);
            _context.Prescriptions.Remove(p);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Đã xoá đơn thuốc.");
        }

        // ✅ 6. Gửi lại email (chỉ nếu đơn thuộc bác sĩ)
        public async Task<ServiceResult<string>> SendPrescriptionEmailForDoctorAsync(int id, int doctorId)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Details)
                .Include(x => x.Appointment).ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id);

            if (p == null)
                return ServiceResult<string>.Fail("Không tìm thấy đơn thuốc.");

            if (p.Appointment.DoctorId != doctorId)
                return ServiceResult<string>.Fail("Bạn không có quyền gửi email đơn thuốc này.");

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

        // Helper gửi email
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
        // ✅ 7. Danh sách bệnh nhân đã khám của bác sĩ
        public async Task<ServiceResult<List<ExaminedPatientDto>>> GetExaminedPatientsForDoctorAsync(int doctorId, string? keyword = null)
        {
            var query = _context.RegistrationRequests
                .Include(r => r.Exam)
                .Include(r => r.Appointment)
                .Where(r => r.Status == "Examined"
                            && r.Appointment != null
                            && r.Appointment.DoctorId == doctorId);

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

    }
}
