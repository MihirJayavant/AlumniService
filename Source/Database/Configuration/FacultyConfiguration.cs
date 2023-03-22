namespace Database.Configuration;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasIndex(f => f.Email)
                                    .IsUnique()
                                    .IncludeProperties(f =>
                                        new { f.FirstName, f.LastName }
                                    );

        builder.Property(f => f.Email)
                                    .HasColumnType("varchar(50)")
                                    .IsRequired();

        builder.Property(f => f.FirstName)
                                    .HasColumnType("varchar(50)")
                                    .IsRequired();

        builder.Property(f => f.LastName)
                                    .HasColumnType("varchar(50)")
                                    .IsRequired();

        builder.Property(e => e.Extension)
                                    .HasColumnType("varchar(10)")
                                    .IsRequired();

        builder.Property(e => e.MobileNo)
                                    .IsRequired();

        builder.Property(e => e.DateCreated)
                                    .IsRequired();

    }
}
