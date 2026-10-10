using Alumni.Api.ExtensionService;

namespace Alumni.Api.Controllers;

public sealed class BootstrapController : IEndpoint
{
    public void Add(IEndpointRouteBuilder app)
        => app.MapPost("/auth/bootstrap", BootstrapAsync)
            .AllowAnonymous()
            .RequireRateLimiting(InvitationRateLimiting.BootstrapPolicy)
            .Produces<AdminBootstrapped>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status429TooManyRequests);

    private static async Task<IResult> BootstrapAsync(
        BootstrapAdmin request,
        BootstrapAdminHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(request, cancellationToken);
        return result.IsT0
            ? Results.Json(result.AsT0, statusCode: StatusCodes.Status201Created)
            : result.ToServerResult();
    }

}
