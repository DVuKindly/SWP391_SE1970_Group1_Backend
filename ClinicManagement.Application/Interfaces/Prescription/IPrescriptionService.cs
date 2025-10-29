using ClinicManagement.Application.DTOS.Request.Prescription;
using ClinicManagement.Application.DTOS.Request.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Prescription
{
    public interface IPrescriptionService
    {
        Task<ServiceResult<List<ExaminedPatientDto>>> GetExaminedPatientsAsync(string? keyword = null);
        Task<ServiceResult<List<PrescriptionResponseDto>>> GetAllPrescriptionsAsync(int? doctorId = null, int? patientId = null);
        Task<ServiceResult<PrescriptionResponseDto>> GetPrescriptionDetailAsync(int prescriptionId);

        Task<ServiceResult<PrescriptionResponseDto>> CreatePrescriptionAsync(PrescriptionRequestDto request, int staffId);
        Task<ServiceResult<PrescriptionResponseDto>> UpdatePrescriptionAsync(int id, PrescriptionRequestDto request, int staffId);
        Task<ServiceResult<bool>> DeletePrescriptionAsync(int id);
        Task<ServiceResult<string>> SendPrescriptionEmailAsync(int id);
        Task<ServiceResult<PatientPaymentSummaryDto>> GetPaymentSummaryAsync(DateTime? from = null, DateTime? to = null);
    }
}
