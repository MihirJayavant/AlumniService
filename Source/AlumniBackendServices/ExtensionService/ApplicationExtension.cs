
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;

namespace AlumniBackendServices.ExtensionService;

public static class ApplicationExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ValidationService).Assembly);
        });
        services.AddAutoMapper(typeof(ValidationService));
        services.AddCors();
        services.AddTransient<IValidationService, ValidationService>();
    }

    public static void UseApplication(this IApplicationBuilder app)
                => app.UseCors(builder => builder.AllowAnyOrigin()
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod());
}
