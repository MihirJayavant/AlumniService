using System.Text;
using System.Text.Json;
using Alumni.Api.Controllers;
using Alumni.Auth.Bootstrap;
using Core;
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
    private const string ValidRequest = """{"email":"admin@example.com","password":"ValidPassword123!"}""";

    [Fact]
    public async Task Add_WhenRegistered_ExposesAnonymousJsonPostWithRateLimit()
    {
        await using var app = CreateApplication(new BootstrapStoreStub());

        var endpoint = GetEndpoint(app);

        Assert.Equal("/auth/bootstrap", endpoint.RoutePattern.RawText);
        Assert.Equal(["POST"], endpoint.Metadata.GetRequiredMetadata<HttpMethodMetadata>().HttpMethods);
        Assert.NotNull(endpoint.Metadata.GetMetadata<IAllowAnonymous>());
        Assert.Equal("admin-bootstrap", endpoint.Metadata.GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName);
        var accepts = endpoint.Metadata.GetRequiredMetadata<IAcceptsMetadata>();
        Assert.Equal(typeof(BootstrapAdmin), accepts.RequestType);
        Assert.Contains("application/json", accepts.ContentTypes);
        Assert.False(accepts.IsOptional);
    }

    [Fact]
    public async Task Bootstrap_WhenSuccessful_ReturnsCreatedAccountWithoutTokensOrPassword()
    {
        var store = new BootstrapStoreStub();
        await using var app = CreateApplication(store);
        var context = CreateContext(app.Services, ValidRequest);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status201Created, context.Response.StatusCode);
        var response = await ReadResponseAsync(context);
        Assert.Equal("admin-account", response.GetProperty("userId").GetString());
        Assert.Equal("admin@example.com", response.GetProperty("email").GetString());
        Assert.Equal(2, response.EnumerateObject().Count());
        Assert.Equal("ValidPassword123!", store.Request?.Password);
        Assert.Equal(TestContext.Current.CancellationToken, store.CancellationToken);
    }

    [Fact]
    public async Task Bootstrap_WhenAlreadyInitialized_ReturnsConflict()
    {
        var store = new BootstrapStoreStub
        {
            Result = new ErrorType { Status = ResponseStatus.Conflict, Message = "Bootstrap is unavailable." }
        };
        await using var app = CreateApplication(store);
        var context = CreateContext(app.Services, ValidRequest);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        var response = await ReadResponseAsync(context);
        Assert.Equal("Bootstrap is unavailable.", response.GetProperty("error").GetString());
    }

    [Theory]
    [InlineData("{malformed")]
    [InlineData("""{"email":"invalid","password":"ValidPassword123!"}""")]
    [InlineData("""{"email":"admin@example.com","password":"short"}""")]
    public async Task Bootstrap_WhenRequestIsInvalid_RejectsWithoutStore(string json)
    {
        var store = new BootstrapStoreStub();
        await using var app = CreateApplication(store);
        var context = CreateContext(app.Services, json);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task Bootstrap_WhenCredentialsAreOnlyInQuery_RejectsWithoutStore()
    {
        var store = new BootstrapStoreStub();
        await using var app = CreateApplication(store);
        var context = CreateContext(app.Services, null);
        context.Request.QueryString = new QueryString("?email=admin@example.com&password=ValidPassword123!");

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task Bootstrap_WhenStoreThrows_SanitizesInternalErrorReturnedByHandler()
    {
        var store = new BootstrapStoreStub { Failure = new InvalidOperationException("sensitive-account-details") };
        await using var app = CreateApplication(store);
        var context = CreateContext(app.Services, ValidRequest);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        var response = await ReadResponseAsync(context);
        Assert.DoesNotContain("sensitive-account-details", response.GetRawText(), StringComparison.Ordinal);
        Assert.DoesNotContain("ValidPassword123!", response.GetRawText(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Bootstrap_WhenStoreCancels_PropagatesCancellation()
    {
        var store = new BootstrapStoreStub { Cancel = true };
        await using var app = CreateApplication(store);
        var context = CreateContext(app.Services, ValidRequest);

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(
            () => GetEndpoint(app).RequestDelegate!(context));

        Assert.Equal(TestContext.Current.CancellationToken, exception.CancellationToken);
    }

    private static WebApplication CreateApplication(BootstrapStoreStub store)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [] });
        builder.Services.AddSingleton<IAdminBootstrapStore>(store);
        builder.Services.AddSingleton<BootstrapAdminHandler>();
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

    private static async Task<JsonElement> ReadResponseAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body,
            cancellationToken: TestContext.Current.CancellationToken);
        return document.RootElement.Clone();
    }

    private sealed class BodyDetectionFeature(bool canHaveBody) : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => canHaveBody;
    }
}
