using Microsoft.AspNetCore.Identity;

namespace Alumni.Auth;

public class AuthUser : IdentityUser
{
    public AccountStatus Status { get; set; } = AccountStatus.PendingActivation;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
