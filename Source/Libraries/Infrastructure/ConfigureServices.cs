using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(ISettingService setting)
        {
            services.AddDbContext<ApplicationContext>(options =>
               options.UseNpgsql(setting.DatabaseSetting.Connection, b => b.MigrationsAssembly("Alumni.Api")));

            services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());
            services.AddScoped<IStudentDbContext>(provider => provider.GetRequiredService<IApplicationContext>());
            services.AddScoped<IFacultyDbContext>(provider => provider.GetRequiredService<IApplicationContext>());
            services.AddScoped<IAuthDbContext>(provider => provider.GetRequiredService<IApplicationContext>());

            services.AddIdentityCore<AuthUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
            }).AddRoles<AuthRole>().AddEntityFrameworkStores<ApplicationContext>();

            services.TryAddSingleton(TimeProvider.System);
            services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("Postgres");

            services.AddScoped<BootstrapAdminHandler>();

            return services;
        }
    }
}
