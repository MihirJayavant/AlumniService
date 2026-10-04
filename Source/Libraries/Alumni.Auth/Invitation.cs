namespace Alumni.Auth;

public sealed class Invitation
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string NormalizedEmail { get; set; }
    public required string RoleId { get; set; }
    public required string UserId { get; set; }
    // Store a SHA-256 digest, never the invitation secret sent by email.
    public required string TokenHash { get; set; }
    public required string InvitedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public uint Version { get; set; }
}
