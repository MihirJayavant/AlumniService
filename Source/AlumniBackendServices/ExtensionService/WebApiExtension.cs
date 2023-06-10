using AlumniBackendServices.Grpc;
using AlumniBackendServices.Services;
using Application;
using Grpc.AspNetCore.Server;

namespace AlumniBackendServices.ExtensionService;

public static class WebApiExtension
{
    public static void AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddCors();
        services.AddSingleton<ISettingService>(new SettingService(configuration));
        services.AddGrpc();
    }

    public static void UseApplication(this WebApplication app)
    {
        app.UseCors(builder => builder.AllowAnyOrigin()
                                                           .AllowAnyHeader()
                                                           .AllowAnyMethod());
    }
}
