using ClinicManagement.Application.DTOS.Common;
using ClinicManagement.Application.DTOS.Request.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces.Booking
{
    public interface IDoctorScheduleService
    {
        Task<List<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);

        Task<List<DoctorWorkPatternDto>> GetWorkPatternsAsync(int doctorId, CancellationToken ct = default);
        Task<List<DoctorAvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date, CancellationToken ct = default);



    }
}
