using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Alumni.Api.UnitTests.ExtensionService;

public class OpenApiOptionsTests
{
    [Fact]
    public void AddApplicationOpenApi_WhenRegistered_UsesStrictJsonNumberHandling()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [] });
        builder.Services.AddApplicationOpenApi(builder);
        using var provider = builder.Services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>().Value;

        Assert.Equal(JsonNumberHandling.Strict, options.SerializerOptions.NumberHandling);
    }
}
