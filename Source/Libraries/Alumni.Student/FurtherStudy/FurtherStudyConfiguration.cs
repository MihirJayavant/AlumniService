using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Student.FurtherStudy;

public class FurtherStudyConfiguration : IEntityTypeConfiguration<FurtherStudy>
{
    public void Configure(EntityTypeBuilder<FurtherStudy> builder)
    {
        builder.Metadata.SetSchema("Student");
        builder.ToTable("further_studies");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedOnAdd();

        builder.Property(f => f.FurtherStudyId).IsRequired();
        builder.HasIndex(c => c.FurtherStudyId).IsUnique();

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
