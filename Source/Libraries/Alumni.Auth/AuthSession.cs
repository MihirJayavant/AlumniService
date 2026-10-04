namespace Alumni.Auth;

public sealed class AuthSession
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    // Associates a device session with an OpenIddict authorization grant.
    public string? AuthorizationId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public uint Version { get; set; }
}
