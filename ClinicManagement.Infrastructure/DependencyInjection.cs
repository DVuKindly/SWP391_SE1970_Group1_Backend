
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Infrastructure.Services.Auth;
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
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Auth Service
            services.AddScoped<IAuthService, AuthService>();

            //  sau này nếu có thêm service khác thì đăng ký tiếp ở đây
  

            return services;
        }
    }
}
