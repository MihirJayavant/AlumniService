using System.Text.Json.Serialization;
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
            builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);
            builder.Services.AddOpenApi();
        }
    }
    extension(WebApplication app)
    {
        public void UseApplicationOpenApi()
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                    options.SwaggerEndpoint("/openapi/v1.json", "Alumni Backend Services"));
                app.MapScalarApiReference();
            }
        }
    }
}
