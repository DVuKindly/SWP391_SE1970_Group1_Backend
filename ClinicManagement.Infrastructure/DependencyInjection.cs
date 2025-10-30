using ClinicManagement.Application.Interfaces.Appoiment;
using ClinicManagement.Application.Interfaces.Booking;
using ClinicManagement.Application.Interfaces.Department;
using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Application.Interfaces.JWT;
using ClinicManagement.Application.Interfaces.Prescription;
using ClinicManagement.Application.Interfaces.Report;
using ClinicManagement.Application.Interfaces.Services.Auth;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Application.Interfaces.Services.Registration;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Appoiment;
using ClinicManagement.Infrastructure.Services.Auth;
using ClinicManagement.Infrastructure.Services.Booking;
using ClinicManagement.Infrastructure.Services.Dashboard;
using ClinicManagement.Infrastructure.Services.Department;
using ClinicManagement.Infrastructure.Services.Email;
using ClinicManagement.Infrastructure.Services.JWT;

using ClinicManagement.Infrastructure.Services.Payment.VNPAY;
using ClinicManagement.Infrastructure.Services.Prescription;
using ClinicManagement.Infrastructure.Services.Registration;
using ClinicManagement.Infrastructure.Services.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security;

namespace ClinicManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
      
            services.AddDbContext<ClinicDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IAdminAccountService, AdminAccountService>();
            services.AddScoped<IStaffAccountService, StaffAccountService>();
            services.AddScoped<IDoctorAccountService, DoctorAccountService>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            services.AddScoped<IStaffPatientService, StaffPatientService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IRevenueService, RevenueService>();
            services.AddScoped<IPatientServices, PatientServices>();
            // JWT & Role Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<RoleService>();

            services.AddSingleton<IEmailService>(sp =>
            {
                var emailCfg = config.GetSection("Email");

                var host = emailCfg["Host"] ?? throw new ArgumentNullException("Email:Host");
                var portStr = emailCfg["Port"];
                var username = emailCfg["Username"] ?? throw new ArgumentNullException("Email:Username");
                var password = emailCfg["Password"] ?? throw new ArgumentNullException("Email:Password");
                var fromName = emailCfg["FromName"] ?? "Clinic";

                int port = 587;
                if (!string.IsNullOrEmpty(portStr) && int.TryParse(portStr, out var p))
                {
                    port = p;
                }

                return new EmailService(host, port, username, password, fromName);
            });

            // ✅ VNPay Payment
            services.Configure<VnPayConfig>(config.GetSection("VNPay"));
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<PaymentService>();



            return services;
        }
    }
}
