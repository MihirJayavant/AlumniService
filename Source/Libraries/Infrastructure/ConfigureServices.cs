using System.Text;
using Application;
using Application.Common.Interfaces;
using Database;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ISettingService setting)
    {
        var connection = string.Format(setting.DatabaseSetting.Connection, setting.DatabaseSetting.Password);

        services.AddDbContext<IApplicationDbContext, ApplicationContext>(options =>
           options.UseNpgsql(connection, b => b.MigrationsAssembly("AlumniBackendServices")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(setting.AuthSetting.Secret)),
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                ValidAudience = setting.AuthSetting.ValidAudience,
                ValidIssuer = setting.AuthSetting.ValidIssuer
            };

        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("StudentAccess", policy => policy.RequireRole("Students"));
            options.AddPolicy("AdminAccess", policy => policy.RequireRole("Admin"));
            options.AddPolicy("SuperAdminAccess", policy => policy.RequireRole("SuperAdmin"));
        });


        services.AddTransient<IIdentityService, IdentityService>();

        return services;
    }
}
