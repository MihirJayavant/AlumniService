using AlumniBackendServices.Services;
using HealthChecks.UI.Client;

namespace AlumniBackendServices.ExtensionService;

public static class WebApiExtension
{
    extension(IServiceCollection services)
    {
        public void AddWebApiServices(IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddCors();
            services.AddSingleton<ISettingService>(new SettingService(configuration));
            services.AddGrpc();
        }
    }

    extension(WebApplication app)
    {
        public void UseApplication()
        {
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            app.MapHealthChecks("/healthz", new()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}
