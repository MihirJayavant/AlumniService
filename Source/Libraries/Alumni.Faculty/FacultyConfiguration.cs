using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Faculty;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.Metadata.SetSchema("Faculty");
        builder.ToTable("faculties");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.FacultyId);
        builder.HasIndex(s => s.FacultyId).IsUnique();

        builder.Property(s => s.Email).HasColumnType("varchar(100)")
                                        .IsRequired();
        builder.HasIndex(s => s.Email).IsUnique();

        builder.Property(s => s.FirstName).HasColumnType("varchar(100)")
                                        .IsRequired();

        builder.Property(s => s.LastName).HasColumnType("varchar(100)")
                                        .IsRequired();

        builder.Property(s => s.Extension)
                        .HasColumnType("varchar(10)")
                        .IsRequired();

        builder.Property(s => s.MobileNo)
                        .IsRequired();

        builder.Property(s => s.CreatedAt)
                        .IsRequired();

        builder.Property(s => s.UpdatedAt)
                        .IsRequired();

    }
}
