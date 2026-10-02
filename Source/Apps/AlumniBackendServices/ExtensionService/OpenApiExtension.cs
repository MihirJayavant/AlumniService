using Scalar.AspNetCore;

namespace AlumniBackendServices.ExtensionService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case",
    Justification = "False positive for multiple C# extension blocks: https://github.com/dotnet/sdk/issues/51716")]
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
