using ClinicManagement.Application.Interfaces.Booking;
using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Application.Interfaces.JWT;
using ClinicManagement.Application.Interfaces.Services.Auth;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Application.Interfaces.Services.Registration;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Auth;
using ClinicManagement.Infrastructure.Services.Booking;
using ClinicManagement.Infrastructure.Services.Dashboard;
using ClinicManagement.Infrastructure.Services.Email;
using ClinicManagement.Infrastructure.Services.JWT;
using ClinicManagement.Infrastructure.Services.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // DbContext
            services.AddDbContext<ClinicDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Auth & Registration Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IAdminAccountService, AdminAccountService>();
            services.AddScoped<IStaffAccountService, StaffAccountService>();
            services.AddScoped<IDoctorAccountService, DoctorAccountService>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            services.AddScoped<IStaffPatientService, StaffPatientService>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<RoleService>();

            // Email Service
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


            return services;
        }
    }
}
