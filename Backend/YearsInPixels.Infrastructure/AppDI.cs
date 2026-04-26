using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YearsInPixels.Application.Configurations;
using YearsInPixels.Application.Interfaces;
using YearsInPixels.Application.Services;
using YearsInPixels.Application.Validators;
using YearsInPixels.Domain.Interfaces;
using YearsInPixels.Infrastructure.Repositories;

namespace YearsInPixels.Infrastructure
{
    public static class AppDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(op => op.UseSqlite(configuration.GetConnectionString("Default")
                ?? throw new ArgumentNullException("No connection string provided"), b => b.MigrationsAssembly("YearsInPixels.Infrastructure")));

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtSettings:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!)),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddHttpContextAccessor();

            services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<DailyPixelDtoValidator>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPixelRepository, PixelRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPixelService, PixelService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
