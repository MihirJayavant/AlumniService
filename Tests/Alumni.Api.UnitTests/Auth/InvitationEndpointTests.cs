using System.Text;
using System.Text.Json;
using Alumni.Api.Controllers;
using Alumni.Auth.Invitations;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class InvitationEndpointTests
{
    [Fact]
    public async Task Add_WhenRegistered_ExposesOnlyAnonymousJsonAcceptance()
    {
        await using var app = CreateApplication(new StubInvitationService());

        var endpoint = GetEndpoint(app);

        Assert.Equal("/auth/invitations/accept", endpoint.RoutePattern.RawText);
        Assert.Equal(["POST"], endpoint.Metadata.GetRequiredMetadata<HttpMethodMetadata>().HttpMethods);
        Assert.NotNull(endpoint.Metadata.GetMetadata<IAllowAnonymous>());
        Assert.Equal("invitation-acceptance",
            endpoint.Metadata.GetRequiredMetadata<EnableRateLimitingAttribute>().PolicyName);
        var accepts = endpoint.Metadata.GetRequiredMetadata<IAcceptsMetadata>();
        Assert.Equal(typeof(AcceptInvitation), accepts.RequestType);
        Assert.Contains("application/json", accepts.ContentTypes);
        Assert.False(accepts.IsOptional);
    }

    [Fact]
    public async Task Accept_WhenSuccessful_ForwardsJsonAndCancellationAndReturnsAccountId()
    {
        var service = new StubInvitationService();
        await using var app = CreateApplication(service);
        using var cancellation = new CancellationTokenSource();
        var context = CreateContext(app.Services,
            """{"token":"invitation-secret","password":"new-password"}""", cancellation.Token);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(new AcceptInvitation("invitation-secret", "new-password"), service.Request);
        Assert.Equal(cancellation.Token, service.CancellationToken);
        Assert.Equal(1, service.Calls);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        var response = await ReadResponseAsync(context);
        Assert.Equal("account-123", response.GetProperty("userId").GetString());
        Assert.Single(response.EnumerateObject());
    }

    [Theory]
    [InlineData("Invalid invitation.")]
    [InlineData("Password does not meet the requirements.")]
    public async Task Accept_WhenServiceRejectsRequest_ReturnsBadRequest(string message)
    {
        var service = new StubInvitationService
        {
            Result = new ErrorType { Status = ResponseStatus.BadRequest, Message = message }
        };
        await using var app = CreateApplication(service);
        var context = CreateContext(app.Services,
            """{"token":"invitation-secret","password":"new-password"}""", TestContext.Current.CancellationToken);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        var response = await ReadResponseAsync(context);
        Assert.Equal(message, response.GetProperty("error").GetString());
        Assert.Single(response.EnumerateObject());
    }

    [Fact]
    public async Task Accept_WhenCredentialsAreOnlyInQuery_RejectsWithoutCallingService()
    {
        var service = new StubInvitationService();
        await using var app = CreateApplication(service);
        var context = CreateContext(app.Services, null, TestContext.Current.CancellationToken);
        context.Request.QueryString = new QueryString("?token=invitation-secret&password=new-password");

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(0, service.Calls);
    }

    [Fact]
    public async Task Accept_WhenServiceIsCancelled_PropagatesCancellation()
    {
        var service = new StubInvitationService { Cancel = true };
        await using var app = CreateApplication(service);
        using var cancellation = new CancellationTokenSource();
        var context = CreateContext(app.Services,
            """{"token":"invitation-secret","password":"new-password"}""", cancellation.Token);

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(
            () => GetEndpoint(app).RequestDelegate!(context));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(1, service.Calls);
    }

    [Fact]
    public async Task Accept_WhenServiceThrows_ReturnsGenericProblemWithoutExceptionDetails()
    {
        const string sensitiveDetails = "Internal account details and invitation-secret";
        var service = new StubInvitationService { Failure = new InvalidOperationException(sensitiveDetails) };
        await using var app = CreateApplication(service);
        var context = CreateContext(app.Services,
            """{"token":"invitation-secret","password":"new-password"}""", TestContext.Current.CancellationToken);

        await GetEndpoint(app).RequestDelegate!(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        var response = await ReadResponseAsync(context);
        Assert.Equal(500, response.GetProperty("status").GetInt32());
        Assert.DoesNotContain(sensitiveDetails, response.GetRawText(), StringComparison.Ordinal);
        Assert.DoesNotContain("invitation-secret", response.GetRawText(), StringComparison.Ordinal);
        Assert.DoesNotContain("new-password", response.GetRawText(), StringComparison.Ordinal);
    }

    private static WebApplication CreateApplication(StubInvitationService service)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [] });
        builder.Services.AddSingleton<IInvitationService>(service);
        var app = builder.Build();
        new InvitationController().Add(app);
        return app;
    }

    private static RouteEndpoint GetEndpoint(WebApplication app)
        => Assert.Single(((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints).OfType<RouteEndpoint>());

    private static DefaultHttpContext CreateContext(IServiceProvider services, string? json,
        CancellationToken cancellationToken = default)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = services,
            RequestAborted = cancellationToken
        };
        context.Request.Method = "POST";
        context.Request.Path = "/auth/invitations/accept";
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
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        return document.RootElement.Clone();
    }

    private sealed class BodyDetectionFeature(bool canHaveBody) : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => canHaveBody;
    }

    private sealed class StubInvitationService : IInvitationService
    {
        public OneOf<InvitationAccepted, ErrorType> Result { get; init; } = new InvitationAccepted("account-123");
        public bool Cancel { get; init; }
        public Exception? Failure { get; init; }
        public AcceptInvitation? Request { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public int Calls { get; private set; }

        public Task<OneOf<InvitationAccepted, ErrorType>> AcceptAsync(AcceptInvitation request,
            CancellationToken cancellationToken = default)
        {
            Request = request;
            CancellationToken = cancellationToken;
            Calls++;
            if (Cancel)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            if (Failure is not null)
            {
                throw Failure;
            }

            return Task.FromResult(Result);
        }

        public Task<OneOf<InvitationIssued, ErrorType>> ResendAsync(string invitedByUserId, Guid invitationId,
            CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Acceptance endpoint must not resend invitations.");

        public Task<OneOf<InvitationRevoked, ErrorType>> RevokeAsync(string invitedByUserId, Guid invitationId,
            CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Acceptance endpoint must not revoke invitations.");
    }
}
