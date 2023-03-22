namespace Database.Configuration;

public class FurtherStudyConfiguration : IEntityTypeConfiguration<FurtherStudy>
{
    public void Configure(EntityTypeBuilder<FurtherStudy> builder)
    {
        builder.HasIndex(f => new { f.InstituteName, f.Country });

        builder.Property(f => f.InstituteName)
                                    .HasColumnType("varchar(50)")
                                    .IsRequired();

        builder.Property(f => f.Degree)
                                    .HasColumnType("varchar(50)")
                                    .IsRequired();

        builder.Property(f => f.Country)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();

        builder.Property(f => f.City)
                                    .HasColumnType("varchar(30)")
                                    .IsRequired();

        builder.Property(f => f.PassingYear)
                                    .HasColumnType("SMALLINT");

        builder.Property(f => f.AdmissionYear)
                                    .HasColumnType("SMALLINT")
                                    .IsRequired();
    }
}
