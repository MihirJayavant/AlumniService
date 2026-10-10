using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace Alumni.Auth;

public sealed class AuthSessionConfiguration : IEntityTypeConfiguration<AuthSession>
{
    public void Configure(EntityTypeBuilder<AuthSession> builder)
    {
        builder.ToTable("Sessions", AuthModelConfiguration.Schema, table =>
        {
            table.HasCheckConstraint("CK_Sessions_Expiry", "\"ExpiresAt\" > \"CreatedAt\"");
            table.HasCheckConstraint("CK_Sessions_LastSeen", "\"LastSeenAt\" >= \"CreatedAt\"");
        });
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Version).IsRowVersion();
        builder.HasIndex(session => new { session.UserId, session.RevokedAt });
        builder.HasIndex(session => session.ExpiresAt);
        builder.HasIndex(session => session.AuthorizationId).IsUnique();
        builder.HasOne<AuthUser>().WithMany().HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<OpenIddictEntityFrameworkCoreAuthorization>().WithMany()
            .HasForeignKey(session => session.AuthorizationId).OnDelete(DeleteBehavior.SetNull);
    }
}
