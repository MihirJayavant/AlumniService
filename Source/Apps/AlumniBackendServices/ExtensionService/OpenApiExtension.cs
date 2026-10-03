using Scalar.AspNetCore;

namespace AlumniBackendServices.ExtensionService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case",
    Justification = "False positive for multiple C# extension blocks: https://github.com/dotnet/sdk/issues/51716")]
public static class OpenApiExtension
{
    extension(IServiceCollection services)
    {
        public void AddApplicationOpenApi(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }
    }
    extension(WebApplication app)
    {
        public void UseApplicationOpenApi()
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapScalarApiReference(options =>
                    options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json"));
            }
        }
    }
}
