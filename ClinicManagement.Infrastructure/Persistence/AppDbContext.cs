using ClinicManagement.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Persistence
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

        // DbSet cho tất cả entity
        public DbSet<Patient> Patients { get; set; } = default!;
        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<Permission> Permissions { get; set; } = default!;
        public DbSet<EmployeeRole> EmployeeRoles { get; set; } = default!;
        public DbSet<RolePermission> RolePermissions { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;
        public DbSet<DoctorDepartment> DoctorDepartments { get; set; } = default!;
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; } = default!;
        public DbSet<Exam> Exams { get; set; } = default!;
        public DbSet<DoctorProfile> DoctorProfiles { get; set; } = default!;
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RegistrationRequest>()
                .HasKey(r => r.RegistrationRequestId);

            modelBuilder.Entity<RegistrationRequest>()
                .HasIndex(r => r.Email); 
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.PatientUserId);

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Email)
                .IsUnique();

          
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeUserId);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<DoctorProfile>()
                .HasOne(dp => dp.Employee)
                .WithOne(e => e.DoctorProfile)
                .HasForeignKey<DoctorProfile>(dp => dp.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
         
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<Permission>()
                .HasKey(p => p.PermissionId);

         
            modelBuilder.Entity<EmployeeRole>()
                .HasKey(er => new { er.EmployeeId, er.RoleId });

            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRoles)
                .HasForeignKey(er => er.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Role)
                .WithMany(r => r.EmployeeRoles)
                .HasForeignKey(er => er.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

          
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>()
                .HasKey(d => d.DepartmentId);

            modelBuilder.Entity<DoctorDepartment>()
                .HasKey(dd => dd.DoctorDepartmentId);

            modelBuilder.Entity<DoctorDepartment>()
                .HasOne(dd => dd.Doctor)
                .WithMany(e => e.DoctorDepartments)
                .HasForeignKey(dd => dd.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorDepartment>()
                .HasOne(dd => dd.Department)
                .WithMany(d => d.DoctorDepartments)
                .HasForeignKey(dd => dd.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exam>()
                .HasKey(e => e.ExamId);

            modelBuilder.Entity<Exam>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(e => e.AppointmentsAsDoctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.CreatedBy)
                .WithMany(e => e.AppointmentsCreated)
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Employee>().HasData(
    new Employee
    {
        EmployeeUserId = 1,
        Email = "admin@gmail.com",
        PasswordHash ="$2a$11$7Pb2XS4fRQWCvUfRhkTNJO2Qib1pTOFjWOX1SQSyIhjNN1CzfXVKC",
        FullName = "Super Admin",
        Phone = "0123456789",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow
    }
);

            modelBuilder.Entity<EmployeeRole>().HasData(
                new EmployeeRole
                {
                    EmployeeId = 1,
                    RoleId = 1,  // RoleId=1 là Admin (đã seed ở trên)
                    AssignedById = null,
                    AssignedAtUtc = DateTime.UtcNow
                }
            );
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ApprovedBy)
                .WithMany(e => e.AppointmentsApproved)
                .HasForeignKey(a => a.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Exam)
                .WithMany()
                .HasForeignKey(a => a.ExamId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<DoctorSchedule>()
                .HasKey(ds => ds.ScheduleId);

            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.Doctor)
                .WithMany(e => e.Schedules)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.CreatedBy)
                .WithMany()
                .HasForeignKey(ds => ds.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

         
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Admin" },
                new Role { RoleId = 2, Name = "Staff_Patient" },
                new Role { RoleId = 3, Name = "Staff_Doctor" },
                new Role { RoleId = 4, Name = "Doctor" }
            );
        }
    }
}
