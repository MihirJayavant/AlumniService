using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Alumni.Auth;

public static class AuthModelConfiguration
{
    public const string Schema = "Auth";

    // Apply after IdentityDbContext.OnModelCreating so Identity's mappings are overridden.
    public static void ConfigureAuthModel(this ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Entity<AuthUser>(user =>
        {
            user.ToTable("AspNetUsers", Schema, table => table.HasCheckConstraint("CK_Users_ActiveProfile",
                "NOT \"IsActive\" OR \"StudentProfileId\" IS NOT NULL OR \"FacultyProfileId\" IS NOT NULL"));
            user.Property(account => account.Email).HasMaxLength(256).IsRequired();
            user.Property(account => account.NormalizedEmail).HasMaxLength(256).IsRequired();
            user.Property(account => account.IsActive).HasDefaultValue(false);
            user.HasIndex(account => account.NormalizedEmail).IsUnique();
            user.HasIndex(account => account.StudentProfileId).IsUnique();
            user.HasIndex(account => account.FacultyProfileId).IsUnique();
        });
        builder.Entity<AuthRole>().ToTable("AspNetRoles", Schema);
        builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", Schema);
        builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", Schema);
        builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", Schema);
        builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", Schema);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", Schema);

        builder.Entity<AuthRole>().HasData(
            CreateRole(AuthRoles.Student),
            CreateRole(AuthRoles.FacultyReader),
            CreateRole(AuthRoles.FacultyEditor),
            CreateRole(AuthRoles.FacultyAdmin));

        builder.UseOpenIddict();
        builder.Entity<OpenIddictEntityFrameworkCoreApplication>().ToTable("Applications", Schema);
        builder.Entity<OpenIddictEntityFrameworkCoreAuthorization>().ToTable("Authorizations", Schema);
        builder.Entity<OpenIddictEntityFrameworkCoreScope>().ToTable("Scopes", Schema);
        builder.Entity<OpenIddictEntityFrameworkCoreToken>().ToTable("Tokens", Schema);

        builder.ApplyConfiguration(new InvitationConfiguration());
        builder.ApplyConfiguration(new AuthSessionConfiguration());
    }

    private static AuthRole CreateRole(string name) => new()
    {
        Id = name,
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = name
    };
}
