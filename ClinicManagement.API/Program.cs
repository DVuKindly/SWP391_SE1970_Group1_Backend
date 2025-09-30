using ClinicManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using DotNetEnv;

namespace ClinicManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
         
            var builder = WebApplication.CreateBuilder(args);

          
            Env.Load("../.env"); 


      
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            // Test in ra để chắc chắn env đã load
            Console.WriteLine("Google ClientId = " + builder.Configuration["Authentication:Google:ClientId"]);
            Console.WriteLine("Google ClientSecret = " + builder.Configuration["Authentication:Google:ClientSecret"]);

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddControllers();
            Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("admin"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Nhập theo dạng: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

            // JWT config
            var jwtCfg = builder.Configuration.GetSection("Jwt");
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtCfg["Issuer"],
                        ValidAudience = jwtCfg["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtCfg["Key"]!)
                        ),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = "sub",
                        RoleClaimType = ClaimTypes.Role
                    };
                })
                // Google OAuth2
                .AddCookie()
                .AddGoogle(options =>
                {
                    var googleCfg = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleCfg["ClientId"] ?? throw new ArgumentNullException("ClientId");
                    options.ClientSecret = googleCfg["ClientSecret"] ?? throw new ArgumentNullException("ClientSecret");
                    options.CallbackPath = "/signin-google";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffDoctorOnly", p => p.RequireRole("Staff_Doctor"));
                options.AddPolicy("StaffPatientOnly", p => p.RequireRole("Staff_Patient"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
