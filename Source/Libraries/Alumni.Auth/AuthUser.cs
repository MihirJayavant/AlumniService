using Microsoft.AspNetCore.Identity;

namespace Alumni.Auth;

public class AuthUser : IdentityUser
{
    public int? StudentProfileId { get; set; }
    public int? FacultyProfileId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
