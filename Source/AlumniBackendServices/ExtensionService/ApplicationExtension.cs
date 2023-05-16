using Application.Students;

namespace AlumniBackendServices.ExtensionService;

public static class ApplicationExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddAutoMapper(typeof(StudentResponse));
        services.AddCors();
    }

    public static void UseApplication(this IApplicationBuilder app)
                => app.UseCors(builder => builder.AllowAnyOrigin()
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod());
}
