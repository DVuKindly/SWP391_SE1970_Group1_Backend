
using ClinicManagement.Application.Interfaces.Services.Auth;
using ClinicManagement.Application.Interfaces.Services.Dashboard;
using ClinicManagement.Application.Interfaces.Services.Registration;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Auth;
using ClinicManagement.Infrastructure.Services.Dashboard;
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

            // Auth Service
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            //  sau này nếu có thêm service khác thì đăng ký tiếp ở đây
            services.AddScoped<IAdminAccountService, AdminAccountService>();
            services.AddScoped<IStaffAccountService, StaffAccountService>();
            services.AddScoped<IDoctorAccountService, DoctorAccountService>();
            return services;
        }
    }
}
