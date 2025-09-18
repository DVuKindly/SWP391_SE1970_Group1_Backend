using ClinicManagement.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<DoctorDepartment> DoctorDepartments => Set<DoctorDepartment>();
        public DbSet<Admin> Admins => Set<Admin>();
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

            // ===== Doctor
            b.Entity<Doctor>(e =>
            {
                e.HasKey(x => x.DoctorId);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Address).HasMaxLength(300);
                e.Property(x => x.Specialization).HasMaxLength(200);
                e.Property(x => x.Image).HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.HasOne(x => x.Role).WithMany(r => r.Doctors).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.SetNull);
            });

            // ===== Patient
            b.Entity<Patient>(e =>
            {
                e.HasKey(x => x.PatientId);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(50).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Address).HasMaxLength(300);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.HasOne(x => x.Role).WithMany(r => r.Patients).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.SetNull);
            });

            // ===== Staff
            b.Entity<Staff>(e =>
            {
                e.HasKey(x => x.StaffId);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(50);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.HasOne(x => x.Role).WithMany(r => r.Staffs).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.SetNull);

                // Seed tài khoản admin mặc định 
           
            });
            b.Entity<Admin>(e =>
            {
                e.HasKey(x => x.AdminId);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(500);
                e.Property(x => x.RefreshToken).HasMaxLength(500);
                e.Property(x => x.RefreshTokenExpiry).HasColumnType("datetime2");
                e.Property(x => x.LastLoginAt).HasColumnType("datetime2");
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(50);

                e.HasData(new Admin
                {
                    AdminId = 1,
                    FullName = "System Administrator",
                    Email = "admin@gmail.com",
                    Phone = "19001898",
                    PasswordHash = "$2a$11$ZNwXzHwf1rJUwxCZ27ygm.QkEItTRnOA.Rw3zuIOOMSp3tM.zhI.q",
                    IsActive = true
                });
            });
            // ===== Exam
            b.Entity<Exam>(e =>
            {
                e.HasKey(x => x.ExamId);
                e.Property(x => x.Examination).HasMaxLength(200).IsRequired();
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                e.HasOne(x => x.Department)
                 .WithMany(d => d.Exams)
                 .HasForeignKey(x => x.DepartmentId)
                 .OnDelete(DeleteBehavior.NoAction);
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
                e.HasData(
                    new Department { DepartmentId = 1, Code = "CARD", Name = "Cardiology", Description = "Khoa Tim mạch", IsActive = true },
                    new Department { DepartmentId = 2, Code = "DERM", Name = "Dermatology", Description = "Khoa Da liễu", IsActive = true },
                    new Department { DepartmentId = 3, Code = "NEUR", Name = "Neurology", Description = "Khoa Thần kinh", IsActive = true },
                    new Department { DepartmentId = 4, Code = "PED", Name = "Pediatrics", Description = "Khoa Nhi", IsActive = true },
                    new Department { DepartmentId = 5, Code = "ORTH", Name = "Orthopedics", Description = "Khoa Chấn thương chỉnh hình", IsActive = true }
                );
            });

            // ===== DoctorDepartment
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

                e.HasIndex(x => x.DoctorId)
                 .IsUnique()
                 .HasFilter("[IsPrimary] = 1")
                 .HasDatabaseName("UX_DoctorDepartments_Primary");

                e.HasIndex(x => x.DepartmentId).HasDatabaseName("IX_DocDept_Department");
            });

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
                e.HasCheckConstraint("CK_App_Status",
                    "Status in ('pending','approved','rejected','cancelled','completed')");
                e.HasIndex(a => new { a.DoctorId, a.AppointmentDate }).HasDatabaseName("IX_App_Doctor_Date");
                e.HasIndex(a => a.PatientId).HasDatabaseName("IX_App_Patient");
            });
        }
    }
}
