using Alumni.Api.ExtensionService;
using Alumni.Auth.Invitations;
using Microsoft.AspNetCore.Mvc;

namespace Alumni.Api.Controllers;

public sealed class InvitationController : IEndpoint
{
    public void Add(IEndpointRouteBuilder app)
        => app.MapPost("/auth/invitations/accept", AcceptAsync)
            .AllowAnonymous()
            .RequireRateLimiting(InvitationRateLimiting.AcceptancePolicy)
            .Produces<InvitationAccepted>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests);

    private static async Task<IResult> AcceptAsync(
        AcceptInvitation request,
        IInvitationService invitations,
        CancellationToken cancellationToken)
    {
        var result = await invitations.AcceptAsync(request, cancellationToken);
        return result.ToServerResult();
    }
}
