using ClinicManagement.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Reflection.Emit;
using System.Security.Principal;

namespace ClinicManagement.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Staff> Staffs => Set<Staff>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<DoctorDepartment> DoctorDepartments => Set<DoctorDepartment>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ===== Role
        b.Entity<Role>(e =>
        {
            e.HasKey(x => x.RoleId);
            e.Property(x => x.Name).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasData(
                 new Role { RoleId = 1, Name = "Admin", IsActive = true },
    new Role { RoleId = 2, Name = "Staff", IsActive = true },        
    new Role { RoleId = 3, Name = "Doctor", IsActive = true },
    new Role { RoleId = 4, Name = "Patient", IsActive = true },
    new Role { RoleId = 5, Name = "Staff_Doctor", IsActive = true },  
    new Role { RoleId = 6, Name = "Staff_Patient", IsActive = true }   
            );
        });

        // ===== Account
        b.Entity<Account>(e =>
        {
            e.HasKey(x => x.AccountId);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(x => x.Role).WithMany(r => r.Accounts).HasForeignKey(x => x.RoleId);

            e.HasOne(x => x.Doctor).WithMany().HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(x => x.Staff).WithMany().HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.NoAction);

            e.HasCheckConstraint("CK_Accounts_OneLink",
                "((CASE WHEN DoctorId IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN PatientId IS NOT NULL THEN 1 ELSE 0 END) + (CASE WHEN StaffId IS NOT NULL THEN 1 ELSE 0 END)) <= 1");
        });

        // ===== Doctor
        b.Entity<Doctor>(e =>
        {
            e.HasKey(x => x.DoctorId);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Address).HasMaxLength(300);
            e.Property(x => x.Specialization).HasMaxLength(200);
            e.Property(x => x.Image).HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // ===== Patient
        b.Entity<Patient>(e =>
        {
            e.HasKey(x => x.PatientId);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Address).HasMaxLength(300);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // ===== Staff
        b.Entity<Staff>(e =>
        {
            e.HasKey(x => x.StaffId);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // ===== Exam
        b.Entity<Exam>(e =>
        {
            e.HasKey(x => x.ExamId);
            e.Property(x => x.Examination).HasMaxLength(200).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // ===== DoctorSchedule
        b.Entity<DoctorSchedule>(e =>
        {
            e.HasKey(x => x.ScheduleId);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne(s => s.Doctor).WithMany(d => d.Schedules).HasForeignKey(s => s.DoctorId);
            e.HasOne(s => s.CreatedByStaff).WithMany().HasForeignKey(s => s.CreatedByStaffId).OnDelete(DeleteBehavior.NoAction);
            e.HasCheckConstraint("CK_Sched_Time", "EndTime > StartTime");
            e.HasIndex(x => new { x.DoctorId, x.StartTime }).HasDatabaseName("IX_Schedule_DoctorTime");
        });
        // ===== Department
        b.Entity<Department>(e =>
        {
            e.HasKey(x => x.DepartmentId);
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // ===== DoctorDepartment (N-N giữa Doctor và Department)
        b.Entity<DoctorDepartment>(e =>
        {
            e.HasKey(x => new { x.DoctorId, x.DepartmentId });
            e.Property(x => x.IsPrimary).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(x => x.Doctor)
             .WithMany(d => d.DoctorDepartments)
             .HasForeignKey(x => x.DoctorId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Department)
             .WithMany(d => d.DoctorDepartments)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.Cascade);

            // Mỗi bác sĩ chỉ có 1 khoa chính
            e.HasIndex(x => x.DoctorId)
             .IsUnique()
             .HasFilter("[IsPrimary] = 1")
             .HasDatabaseName("UX_DoctorDepartments_Primary");

            e.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_DocDept_Department");
        });

        // ===== Exam (map về Department)
        b.Entity<Exam>(e =>
        {
            // phần bạn có sẵn...
            e.HasOne(x => x.Department)
             .WithMany(d => d.Exams)
             .HasForeignKey(x => x.DepartmentId)
             .OnDelete(DeleteBehavior.NoAction);
        });
        // ===== Seed Department (chuyên khoa mẫu)
        b.Entity<Department>().HasData(
            new Department { DepartmentId = 1, Code = "CARD", Name = "Cardiology", Description = "Khoa Tim mạch", IsActive = true },
            new Department { DepartmentId = 2, Code = "DERM", Name = "Dermatology", Description = "Khoa Da liễu", IsActive = true },
            new Department { DepartmentId = 3, Code = "NEUR", Name = "Neurology", Description = "Khoa Thần kinh", IsActive = true },
            new Department { DepartmentId = 4, Code = "PED", Name = "Pediatrics", Description = "Khoa Nhi", IsActive = true },
            new Department { DepartmentId = 5, Code = "ORTH", Name = "Orthopedics", Description = "Khoa Chấn thương chỉnh hình", IsActive = true }
        );
        // ===== Seed Admin account
        b.Entity<Account>().HasData(
            new Account
            {
                AccountId = 1, 
                Email = "admin@gmail.com",
                PasswordHash = "$2a$11$FSviN3EHy9QUYTX.eQcEP.Rr1Ihi5PpJGlnvYfJtbsi3WYQ9Ifgly", 
                RoleId = 1, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );


        // ===== Appointment
        b.Entity<Appointment>(e =>
        {
            e.HasKey(x => x.AppointmentId);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("pending");
            e.HasOne(a => a.Schedule).WithMany().HasForeignKey(a => a.ScheduleId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Doctor).WithMany(d => d.Appointments).HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Patient).WithMany(p => p.Appointments).HasForeignKey(a => a.PatientId);
            e.HasOne(a => a.Exam).WithMany().HasForeignKey(a => a.ExamId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.ApprovedByDoctor).WithMany().HasForeignKey(a => a.ApprovedByDoctorId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.CancelledByAccount).WithMany().HasForeignKey(a => a.CancelledByAccountId).OnDelete(DeleteBehavior.NoAction);

            e.HasCheckConstraint("CK_App_Status",
                 "Status in ('pending','approved','rejected','cancelled','completed')");
            e.HasIndex(a => new { a.DoctorId, a.AppointmentDate }).HasDatabaseName("IX_App_Doctor_Date");
            e.HasIndex(a => a.PatientId).HasDatabaseName("IX_App_Patient");
        });
    }
}
