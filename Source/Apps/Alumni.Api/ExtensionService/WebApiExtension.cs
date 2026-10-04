using Alumni.Api.Services;
using HealthChecks.UI.Client;

namespace Alumni.Api.ExtensionService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case",
    Justification = "False positive for multiple C# extension blocks: https://github.com/dotnet/sdk/issues/51716")]
public static class WebApiExtension
{
    extension(IServiceCollection services)
    {
        public void AddWebApiServices(IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddCors();
            services.AddSingleton<ISettingService>(new SettingService(configuration));
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
