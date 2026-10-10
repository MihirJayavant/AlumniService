using Microsoft.AspNetCore.Identity;

namespace Alumni.Auth;

public interface IAuthDbContext
{
    public DbSet<AuthUser> Users { get; }
    public DbSet<AuthRole> Roles { get; }
    public DbSet<IdentityUserRole<string>> UserRoles { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
