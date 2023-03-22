using AlumniBackendServices.Options;
using Microsoft.OpenApi.Models;

namespace AlumniBackendServices.ExtensionService;

public static class SwaggerExtension
{
    public static void AddApplicationSwagger(this IServiceCollection services, IConfiguration configuration)
    {

        var swaggerOptions = new SwaggerOptions();
        configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

        services.AddSwaggerGen(option =>
        {
            var info = new OpenApiInfo
            {
                Title = swaggerOptions.Description,
                Version = swaggerOptions.Version
            };

            option.SwaggerDoc(swaggerOptions.Version, info);
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

        });
    }

    public static void UseApplicationSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var swaggerOptions = new SwaggerOptions();
        configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger(option => option.RouteTemplate = swaggerOptions.JsonRoute);

        // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(
            option => option
                        .SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description)
            );
    }
}
