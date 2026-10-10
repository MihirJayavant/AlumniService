using System.Text;
using Alumni.Api.Controllers;
using Alumni.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class BootstrapEndpointTests
{
    [Fact]
    public async Task Add_WhenRegistered_ExposesAnonymousJsonPostWithRateLimit()
    {
        await using var app = CreateApplication(new UnexpectedAuthDbContext());

        var endpoint = GetEndpoint(app);

        Assert.Equal("/auth/bootstrap", endpoint.RoutePattern.RawText);
        Assert.Equal(["POST"], endpoint.Metadata.GetRequiredMetadata<HttpMethodMetadata>().HttpMethods);
        Assert.NotNull(endpoint.Metadata.GetMetadata<IAllowAnonymous>());
        Assert.Equal("admin-bootstrap", endpoint.Metadata.GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName);
        var accepts = endpoint.Metadata.GetRequiredMetadata<IAcceptsMetadata>();
        Assert.Equal(typeof(BootstrapAdmin), accepts.RequestType);
        Assert.Contains("application/json", accepts.ContentTypes);
        Assert.False(accepts.IsOptional);
        Assert.Contains(endpoint.Metadata.GetOrderedMetadata<IProducesResponseTypeMetadata>(),
            response => response.StatusCode == StatusCodes.Status201Created
                && response.Type == typeof(AdminBootstrapped));
    }

    [Theory]
    [InlineData("{malformed")]
    [InlineData("""{"email":"invalid","password":"ValidPassword123!"}""")]
    [InlineData("""{"email":"admin@example.com","password":"short"}""")]
    public async Task Bootstrap_WhenRequestIsInvalid_RejectsWithoutPersistence(string json)
    {
        var persistence = new UnexpectedAuthDbContext();
        await using var app = CreateApplication(persistence);
        var context = CreateContext(app.Services, json);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(0, persistence.Accesses);
    }

    [Fact]
    public async Task Bootstrap_WhenCredentialsAreOnlyInQuery_RejectsWithoutPersistence()
    {
        var persistence = new UnexpectedAuthDbContext();
        await using var app = CreateApplication(persistence);
        var context = CreateContext(app.Services, null);
        context.Request.QueryString = new QueryString("?email=admin@example.com&password=ValidPassword123!");

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(0, persistence.Accesses);
    }

    private static WebApplication CreateApplication(UnexpectedAuthDbContext persistence)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [] });
        builder.Services.AddSingleton(BootstrapAdminTests.CreateHandler(persistence));
        var app = builder.Build();
        new BootstrapController().Add(app);
        return app;
    }

    private static RouteEndpoint GetEndpoint(WebApplication app)
        => Assert.Single(((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints).OfType<RouteEndpoint>());

    private static DefaultHttpContext CreateContext(IServiceProvider services, string? json)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = services,
            RequestAborted = TestContext.Current.CancellationToken
        };
        context.Request.Method = "POST";
        context.Request.Path = "/auth/bootstrap";
        context.Response.Body = new MemoryStream();
        context.Features.Set<IHttpRequestBodyDetectionFeature>(new BodyDetectionFeature(json is not null));
        if (json is not null)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            context.Request.ContentType = "application/json";
            context.Request.ContentLength = bytes.Length;
            context.Request.Body = new MemoryStream(bytes);
        }

        return context;
    }

    private sealed class BodyDetectionFeature(bool canHaveBody) : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => canHaveBody;
    }
}
