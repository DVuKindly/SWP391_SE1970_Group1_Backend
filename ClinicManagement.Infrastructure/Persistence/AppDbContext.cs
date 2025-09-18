using ClinicManagement.Domain.Entity;
using ClinicManagement.Domain.Entity.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ClinicManagement.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ================= DbSets =================
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<DoctorDepartment> DoctorDepartments => Set<DoctorDepartment>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ================= Role =================
            b.Entity<Role>(e =>
            {
                e.HasKey(x => x.RoleId);
                e.HasIndex(x => x.Name).IsUnique();

                // Many-to-Many Role <-> Permission
                e.HasMany(r => r.Permissions)
                 .WithMany(p => p.Roles)
                 .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId");
                        j.ToTable("RolePermissions");
                    });
            });

            // ================= Permission =================
            b.Entity<Permission>(e =>
            {
                e.HasKey(x => x.PermissionId);
                e.HasIndex(x => x.Name).IsUnique();
            });

            // ================= Patient =================
            b.Entity<Patient>(e =>
            {
                e.HasKey(x => x.PatientUserId);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasMany(x => x.Appointments)
                 .WithOne(a => a.Patient)
                 .HasForeignKey(a => a.PatientId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ================= Employee =================
            b.Entity<Employee>(e =>
            {
                e.HasKey(x => x.EmployeeUserId);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.IsActive).HasDefaultValue(true);

                // Many-to-Many Employee <-> Role
                e.HasMany(emp => emp.Roles)
                 .WithMany(role => role.Employees)
                 .UsingEntity<Dictionary<string, object>>(
                    "EmployeeRole",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("EmployeeId", "RoleId");
                        j.ToTable("EmployeeRoles");
                    });

                // Lịch bác sĩ
                e.HasMany(x => x.Schedules)
                 .WithOne(s => s.Doctor)
                 .HasForeignKey(s => s.DoctorId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ================= Department =================
            b.Entity<Department>(e =>
            {
                e.HasKey(x => x.DepartmentId);
                e.HasIndex(x => x.Code).IsUnique();
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasMany(d => d.Exams)
                 .WithOne(e => e.Department)
                 .HasForeignKey(e => e.DepartmentId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ================= DoctorDepartment =================
            b.Entity<DoctorDepartment>(e =>
            {
                e.HasKey(x => x.DoctorDepartmentId);

                e.HasOne(x => x.Doctor)
                 .WithMany()
                 .HasForeignKey(x => x.DoctorId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Department)
                 .WithMany(d => d.DoctorDepartments)
                 .HasForeignKey(x => x.DepartmentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Bác sĩ chỉ có 1 khoa chính
                e.HasIndex(x => x.DoctorId)
                 .IsUnique()
                 .HasFilter("[IsPrimary] = 1")
                 .HasDatabaseName("UX_DoctorDepartments_Primary");
            });

            // ================= Exam =================
            b.Entity<Exam>(e =>
            {
                e.HasKey(x => x.ExamId);
                e.Property(x => x.IsActive).HasDefaultValue(true);
            });

            // ================= DoctorSchedule =================
            b.Entity<DoctorSchedule>(e =>
            {
                e.HasKey(x => x.ScheduleId);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasOne(x => x.CreatedBy)
                 .WithMany()
                 .HasForeignKey(x => x.CreatedById)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasCheckConstraint("CK_Schedule_Time", "[EndTime] > [StartTime]");
            });

            // ================= Appointment =================
            b.Entity<Appointment>(e =>
            {
                e.HasKey(x => x.AppointmentId);
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

                e.HasOne(a => a.Patient)
                 .WithMany(p => p.Appointments)
                 .HasForeignKey(a => a.PatientId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.Doctor)
                 .WithMany()
                 .HasForeignKey(a => a.DoctorId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.CreatedBy)
                 .WithMany()
                 .HasForeignKey(a => a.CreatedById)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(a => a.ApprovedBy)
                 .WithMany()
                 .HasForeignKey(a => a.ApprovedById)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(a => a.Exam)
                 .WithMany()
                 .HasForeignKey(a => a.ExamId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasCheckConstraint("CK_Appointment_Time", "[EndTime] > [StartTime]");
            });
        }
    }
}
