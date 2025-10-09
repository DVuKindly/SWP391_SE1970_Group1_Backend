using ClinicManagement.Application.DTOS.Common;
using ClinicManagement.Application.DTOS.Request.Booking;
using ClinicManagement.Application.Interfaces.Booking;
using ClinicManagement.Domain.Entity;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Services.Booking
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly ClinicDbContext _ctx;

        public DoctorScheduleService(ClinicDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<DoctorScheduleDto>> GetDoctorScheduleAsync(
      int doctorId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
        {
            from ??= DateTime.Today;
            to ??= from.Value.AddDays(7);

            // 1️⃣ Lấy danh sách DoctorSchedules thật trong DB
            var schedules = await _ctx.DoctorSchedules
                .Where(s => s.DoctorId == doctorId && s.StartTime >= from && s.EndTime <= to)
                .ToListAsync(ct);

            if (schedules.Any())
            {
                return schedules.Select(s => new DoctorScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    DoctorId = s.DoctorId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Note = s.Note,
                    IsActive = s.IsActive
                }).ToList();
            }

            // 2️⃣ Nếu chưa có -> generate từ DoctorWorkPattern
            var patterns = await _ctx.DoctorWorkPatterns
                .Where(p => p.DoctorId == doctorId && p.IsWorking)
                .ToListAsync(ct);

            var result = new List<DoctorScheduleDto>();

            foreach (var day in EachDay(from.Value, to.Value))
            {
                var dow = (int)day.DayOfWeek; // 0=Sunday
                var pattern = patterns.FirstOrDefault(p => (int)p.DayOfWeek == dow);
                if (pattern == null) continue;

                result.Add(new DoctorScheduleDto
                {
                    DoctorId = doctorId,
                    StartTime = day.Date + pattern.StartTime,
                    EndTime = day.Date + pattern.EndTime,
                    Note = "Generated from pattern",
                    IsActive = true
                });
            }

            return result;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
                yield return day;
        }
        public async Task<List<DoctorAvailableSlotDto>> GetAvailableSlotsAsync(
        int doctorId, DateTime date, CancellationToken ct = default)
        {
            var result = new List<DoctorAvailableSlotDto>();
            var dayOfWeek = (int)date.DayOfWeek;

            // 1️⃣ Lấy tất cả pattern làm việc của ngày đó
            var patterns = await _ctx.DoctorWorkPatterns
                .Where(p => p.DoctorId == doctorId && (int)p.DayOfWeek == dayOfWeek && p.IsWorking)
                .ToListAsync(ct);

            if (!patterns.Any()) return result;

            // 2️⃣ Lấy lịch đã đặt trong ngày
            var appointments = await _ctx.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.StartTime.Date == date.Date &&
                            a.Status != AppointmentStatus.Cancelled &&
                            a.Status != AppointmentStatus.Rejected)
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync(ct);

            // 3️⃣ Sinh slot cho từng pattern
            foreach (var pattern in patterns)
            {
                var slotMinutes = pattern.SlotMinutes == 0 ? 30 : pattern.SlotMinutes;
                var start = date.Date + pattern.StartTime;
                var end = date.Date + pattern.EndTime;

                for (var t = start; t < end; t = t.AddMinutes(slotMinutes))
                {
                    var slotEnd = t.AddMinutes(slotMinutes);

                    bool isBooked = appointments.Any(a =>
                        a.StartTime < slotEnd && a.EndTime > t);

                    result.Add(new DoctorAvailableSlotDto
                    {
                        StartTime = t,
                        EndTime = slotEnd,
                        IsBooked = isBooked
                    });
                }
            }

            return result
                .OrderBy(r => r.StartTime)
                .ToList();
        }




        public async Task<List<DoctorWorkPatternDto>> GetWorkPatternsAsync(
            int doctorId, CancellationToken ct = default)
        {
            var patterns = await _ctx.DoctorWorkPatterns
                .Where(wp => wp.DoctorId == doctorId)
                .OrderBy(wp => wp.DayOfWeek)
                .Select(wp => new DoctorWorkPatternDto
                {
                    WorkPatternId = wp.WorkPatternId,
                    DoctorId = wp.DoctorId,
                    DayOfWeek = wp.DayOfWeek,
                    StartTime = wp.StartTime,
                    EndTime = wp.EndTime,
                    IsWorking = wp.IsWorking,
                    Note = wp.Note
                })
                .ToListAsync(ct);

            return patterns;
        }
    }
}
