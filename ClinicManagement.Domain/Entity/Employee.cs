using ClinicManagement.Domain.Entity.Common;
using ClinicManagement.Domain.Entity;

public class Employee : BaseEntity
{
    public int EmployeeUserId { get; set; }

    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    public string FullName { get; set; } = default!;
    public string? Phone { get; set; }

    // Dùng cho bác sĩ (nullable)
    public string? Specialization { get; set; }
    public string? Image { get; set; }

    // Quan hệ RBAC
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
}
