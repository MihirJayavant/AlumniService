using AlumniBackendServices.Services;
using Application;

namespace AlumniBackendServices.ExtensionService;

public static class WebApiExtension
{
    public static void AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddCors();
        services.AddSingleton<ISettingService>(new SettingService(configuration));
    }

    public static void UseApplication(this IApplicationBuilder app)
                => app.UseCors(builder => builder.AllowAnyOrigin()
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod());
}
