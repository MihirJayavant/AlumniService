namespace Application.Identity;

public sealed record IdentityRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
