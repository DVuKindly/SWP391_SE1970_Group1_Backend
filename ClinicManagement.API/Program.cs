using ClinicManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace ClinicManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Infrastructure (DbContext, Services, etc.)
            builder.Services.AddInfrastructure(builder.Configuration);

            // 2) Controllers
            builder.Services.AddControllers();

            // 3) Swagger + Bearer
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

                var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };
                c.AddSecurityRequirement(securityRequirement);
            });


            // 4) AuthN: JWT Bearer
            var jwtCfg = builder.Configuration.GetSection("Jwt");
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // đặt true trên môi trường production có HTTPS
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

                        // Map claim types cho .NET
                        NameClaimType = "sub",                 // bạn đang đặt sub = AccountId
                        RoleClaimType = ClaimTypes.Role        // bạn đã phát claim role bằng ClaimTypes.Role
                    };

                    // Nếu bạn muốn giữ nguyên tên claim gốc (không map), bật dòng dưới:
                    // options.MapInboundClaims = false;
                });

            // 5) AuthZ: Roles/Policies (tuỳ chọn)
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StaffDoctorOnly", p => p.RequireRole("Staff_Doctor"));
                options.AddPolicy("StaffPatientOnly", p => p.RequireRole("Staff_Patient"));
        
            });


            var app = builder.Build();

            // 6) Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // phải trước UseAuthorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
