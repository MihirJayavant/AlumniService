using Scalar.AspNetCore;

namespace AlumniBackendServices.ExtensionService;

public static class OpenApiExtension
{
    public static void AddApplicationOpenApi(this IServiceCollection services) => services.AddOpenApi("alumni");

    public static void UseApplicationOpenApi(this WebApplication app)
    {
        app.MapOpenApi();

        const string title = "Alumni Backend Services";
        const string path = "/openapi/alumni.json";

        app.MapScalarApiReference(options =>
        {
            options.Title = title;
            options.OpenApiRoutePattern = path;
        });

        app.UseSwaggerUI(
            option => option.SwaggerEndpoint(path, title)
            );
    }
}
