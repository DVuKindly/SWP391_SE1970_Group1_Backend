using ClinicManagement.Application.Interfaces.Email;
using ClinicManagement.Application.Interfaces.Services.Auth;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Application.Interfaces.Services.Registration;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Auth;
using ClinicManagement.Infrastructure.Services.Dashboard;
using ClinicManagement.Infrastructure.Services.Email;
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

            // Email Service
            services.AddSingleton<IEmailService>(sp =>
                new EmailService(
                    host: config["EMAIL_HOST"]!,               
                    port: int.Parse(config["EMAIL_PORT"]!),      
                    from: config["EMAIL_USERNAME"]!,             
                    password: config["EMAIL_PASSWORD"]!,         
                    fromName: config["EMAIL_FROM_NAME"]!         
                ));

            return services;
        }
    }
}
