using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(ISettingService setting)
        {
            services.AddDbContext<IApplicationContext, ApplicationContext>(options =>
               options.UseNpgsql(setting.DatabaseSetting.Connection, b => b.MigrationsAssembly("Alumni.Api")));

            services.AddScoped<IStudentDbContext>(provider => provider.GetRequiredService<IApplicationContext>());
            services.AddScoped<IFacultyDbContext>(provider => provider.GetRequiredService<IApplicationContext>());

            services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("Postgres");

            return services;
        }
    }
}
