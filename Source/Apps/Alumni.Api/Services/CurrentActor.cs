using Alumni.Auth;

namespace Alumni.Api.Services;

/// <summary>Reads the principal shared by REST, GraphQL resolver scopes, and gRPC calls.</summary>
public sealed class CurrentActor(IHttpContextAccessor httpContextAccessor) : ICurrentActor
{
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identities
        .Any(identity => identity.IsAuthenticated) == true;

    public string? UserId => httpContextAccessor.HttpContext?.User.Identities
        .Where(identity => identity.IsAuthenticated)
        .SelectMany(identity => identity.FindAll(AuthClaimTypes.Subject))
        .Select(claim => claim.Value)
        .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
}
