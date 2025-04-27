namespace Application.Identity;

public sealed record IdentityResponse
{
    public required string Email { get; init; }
    public required string Token { get; init; }
}
