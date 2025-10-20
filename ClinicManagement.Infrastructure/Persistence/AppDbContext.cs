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
            public DbSet<DoctorWorkPattern> DoctorWorkPatterns { get; set; } = default!;
            public DbSet<DoctorLeave> DoctorLeaves { get; set; } = default!;
            public DbSet<WorkPatternTemplate> WorkPatternTemplates { get; set; } = default!;
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = default!;

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
                modelBuilder.Entity<WorkPatternTemplate>()
        .HasKey(w => w.Id);


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
                modelBuilder.Entity<DoctorWorkPattern>()
        .HasKey(wp => wp.WorkPatternId);

                modelBuilder.Entity<DoctorWorkPattern>()
                    .HasOne(wp => wp.Doctor)
                    .WithMany(e => e.WorkPatterns)
                    .HasForeignKey(wp => wp.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<DoctorLeave>()
                    .HasKey(dl => dl.LeaveId);

                modelBuilder.Entity<DoctorLeave>()
                    .HasOne(dl => dl.Doctor)
                    .WithMany(e => e.Leaves)
                    .HasForeignKey(dl => dl.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
         .HasKey(p => p.TransactionId);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(p => p.RegistrationRequest)
                .WithMany() 
                .HasForeignKey(p => p.RegistrationRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(p => p.Appointment)
                .WithMany()
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);


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

                modelBuilder.Entity<Department>().HasData(
        new Department
        {
            DepartmentId = 1,
            Code = "CARD",
            Name = "Cardiology",
            Description = "Khoa Tim mạch",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        },
        new Department
        {
            DepartmentId = 2,
            Code = "NEUR",
            Name = "Neurology",
            Description = "Khoa Thần kinh",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        },
        new Department
        {
            DepartmentId = 3,
            Code = "DERM",
            Name = "Dermatology",
            Description = "Khoa Da liễu",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        }
    );

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
            modelBuilder.Entity<RegistrationRequest>()
    .HasOne(r => r.HandledBy)
    .WithMany()
    .HasForeignKey(r => r.HandledById)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RegistrationRequest>()
                .HasOne(r => r.Appointment)
                .WithMany()
                .HasForeignKey(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<RegistrationRequest>()
    .HasOne(r => r.Exam)
    .WithMany()
    .HasForeignKey(r => r.ExamId)
    .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Role>().HasData(
                    new Role { RoleId = 1, Name = "Admin" },
                    new Role { RoleId = 2, Name = "Staff_Patient" },
                    new Role { RoleId = 3, Name = "Staff_Doctor" },
                    new Role { RoleId = 4, Name = "Doctor" }
                );
                modelBuilder.Entity<WorkPatternTemplate>().HasData(
                new WorkPatternTemplate { Id = 1, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0), SlotMinutes = 60, IsActive = true },
                new WorkPatternTemplate { Id = 2, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(17, 0, 0), SlotMinutes = 60, IsActive = true },

                new WorkPatternTemplate { Id = 3, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0), SlotMinutes = 60, IsActive = true },
                new WorkPatternTemplate { Id = 4, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(17, 0, 0), SlotMinutes = 60, IsActive = true },

                // ... repeat for Wed, Thu, Fri, Sat (ids continuing)
                new WorkPatternTemplate { Id = 11, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0), SlotMinutes = 60, IsActive = true },
                new WorkPatternTemplate { Id = 12, DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(17, 0, 0), SlotMinutes = 60, IsActive = true }
            );
            modelBuilder.Entity<Exam>().HasData(
    new Exam
    {
        ExamId = 1,
        Name = "Khám Da liễu cơ bản",
        Description = "Tư vấn và khám da liễu tổng quát, không bao gồm xét nghiệm",
        Price = 200000,
        DepartmentId = 3,
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow
    },
    new Exam
    {
        ExamId = 2,
        Name = "Khám Tim mạch chuyên sâu",
        Description = "Kiểm tra nhịp tim, đo ECG, siêu âm tim",
        Price = 500000,
        DepartmentId = 1,
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow
    },
    new Exam
    {
        ExamId = 3,
        Name = "Khám Thần kinh tổng quát",
        Description = "Khám lâm sàng, đánh giá triệu chứng thần kinh, tư vấn điều trị",
        Price = 400000,
        DepartmentId = 2,
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow
    }
);


        }
    }
    }
