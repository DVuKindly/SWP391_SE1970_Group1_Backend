using ClinicManagement.Application;
using ClinicManagement.Application.DTOS.Request.Dashboard.Patient;

using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Dashboard
{
    public class PatientServices : IPatientServices
    {
        private readonly ClinicDbContext _context;

        public PatientServices(ClinicDbContext context)
        {
            _context = context;
        }

     
        public async Task<ServiceResult<PatientProfileDto>> GetProfileAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return ServiceResult<PatientProfileDto>.Fail("Không tìm thấy bệnh nhân.");

            var dto = new PatientProfileDto
            {
                PatientId = patient.PatientUserId,
                FullName = patient.FullName,
                Email = patient.Email,
                Phone = patient.Phone,
               
        
         
            };

            return ServiceResult<PatientProfileDto>.Ok(dto);
        }

        //  Cập nhật hồ sơ
        public async Task<ServiceResult<bool>> UpdateProfileAsync(int patientId, UpdatePatientProfileDto dto)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return ServiceResult<bool>.Fail("Không tìm thấy bệnh nhân.");

            patient.FullName = dto.FullName ?? patient.FullName;
            patient.Phone = dto.Phone ?? patient.Phone;
            patient.Address = dto.Address ?? patient.Address;
            patient.DOB = dto.DOB;

            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Cập nhật hồ sơ thành công.");
        }

        //  Danh sách lịch hẹn
        public async Task<ServiceResult<List<PatientAppointmentDto>>> GetMyAppointmentsAsync(int patientId)
        {
            var list = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Exam)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartTime)
                .Select(a => new PatientAppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorName = a.Doctor.FullName,
                    ExamName = a.Exam != null ? a.Exam.Name : "(Chưa có)",
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status.ToString(),
                    PaymentMethod = a.PaymentMethod,
                    IsPaid = a.IsPaid,
                    TotalFee = a.TotalFee
                })
                .ToListAsync();

            return ServiceResult<List<PatientAppointmentDto>>.Ok(list);
        }

        //  Chi tiết lịch hẹn
        public async Task<ServiceResult<PatientAppointmentDto>> GetAppointmentDetailAsync(int appointmentId)
        {
            var a = await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.Exam)
                .Include(x => x.Patient)
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);

            if (a == null) return ServiceResult<PatientAppointmentDto>.Fail("Không tìm thấy lịch hẹn.");

            var dto = new PatientAppointmentDto
            {
                AppointmentId = a.AppointmentId,
                DoctorName = a.Doctor.FullName,
                ExamName = a.Exam?.Name ?? "(Chưa có)",
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status.ToString(),
                TotalFee = a.TotalFee,
                PaymentMethod = a.PaymentMethod,
                IsPaid = a.IsPaid
            };

            return ServiceResult<PatientAppointmentDto>.Ok(dto);
        }

        //  Danh sách đơn thuốc
 
        public async Task<ServiceResult<List<PatientPrescriptionDto>>> GetMyPrescriptionsAsync(int patientId)
        {
            var list = await _context.Prescriptions
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Details)
                .Where(p => p.Appointment.PatientId == patientId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .Select(p => new PatientPrescriptionDto
                {
                    PrescriptionId = p.PrescriptionId,
                    DoctorName = p.Appointment.Doctor.FullName,
                    Diagnosis = p.Diagnosis,
                    Note = p.Note,
                    CreatedAt = p.CreatedAtUtc,

                  
                    Medicines = p.Details.Select(d => new PrescriptionMedicineItemDto
                    {
                        MedicineName = d.MedicineName,
                        Dosage = d.Dosage,
                        Frequency = d.Frequency,
                        Duration = d.Duration,
                        Instruction = d.Instruction
                    }).ToList()
                })
                .ToListAsync();

            return ServiceResult<List<PatientPrescriptionDto>>.Ok(list);
        }


        // Chi tiết đơn thuốc
        public async Task<ServiceResult<PatientPrescriptionDto>> GetPrescriptionDetailAsync(int prescriptionId)
        {
            var p = await _context.Prescriptions
                .Include(x => x.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.PrescriptionId == prescriptionId);

            if (p == null)
                return ServiceResult<PatientPrescriptionDto>.Fail("Không tìm thấy đơn thuốc.");

            var dto = new PatientPrescriptionDto
            {
                PrescriptionId = p.PrescriptionId,
                DoctorName = p.Appointment.Doctor.FullName,
                Diagnosis = p.Diagnosis,
                Note = p.Note,
                CreatedAt = p.CreatedAtUtc,
                Medicines = p.Details.Select(d => new PrescriptionMedicineItemDto
                {
                    MedicineName = d.MedicineName,
                    Dosage = d.Dosage,
                    Frequency = d.Frequency,
                    Duration = d.Duration,
                    Instruction = d.Instruction
                }).ToList()
            };

            return ServiceResult<PatientPrescriptionDto>.Ok(dto);
        }
    }
}
