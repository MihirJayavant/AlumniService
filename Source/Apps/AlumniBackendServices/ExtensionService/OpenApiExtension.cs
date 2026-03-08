using Scalar.AspNetCore;

namespace AlumniBackendServices.ExtensionService;

public static class OpenApiExtension
{
    extension(IServiceCollection services)
    {
        public void AddApplicationOpenApi() => services.AddOpenApi("alumni");
    }

    extension(WebApplication app)
    {
        public void UseApplicationOpenApi()
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
}
