using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Auth;

public sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("Invitations", AuthModelConfiguration.Schema, table =>
        {
            table.HasCheckConstraint("CK_Invitations_Expiry", "\"ExpiresAt\" > \"CreatedAt\"");
            table.HasCheckConstraint("CK_Invitations_Acceptance",
                "\"AcceptedAt\" IS NULL OR \"RevokedAt\" IS NULL");
        });
        builder.HasKey(invitation => invitation.Id);
        builder.Property(invitation => invitation.Email).HasMaxLength(256).IsRequired();
        builder.Property(invitation => invitation.NormalizedEmail).HasMaxLength(256).IsRequired();
        builder.Property(invitation => invitation.TokenHash).HasMaxLength(64).IsRequired();
        builder.Property(invitation => invitation.Version).IsRowVersion();
        builder.HasIndex(invitation => invitation.TokenHash).IsUnique();
        builder.HasIndex(invitation => invitation.NormalizedEmail);
        builder.HasIndex(invitation => invitation.ExpiresAt);
        builder.HasOne<AuthRole>().WithMany().HasForeignKey(invitation => invitation.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<AuthUser>().WithMany().HasForeignKey(invitation => invitation.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<AuthUser>().WithMany().HasForeignKey(invitation => invitation.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
