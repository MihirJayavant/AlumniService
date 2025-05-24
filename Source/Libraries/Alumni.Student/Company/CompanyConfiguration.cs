using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Student.Company;

public class CompanyConfiguration : IEntityTypeConfiguration<CompanyEntity>
{
    public void Configure(EntityTypeBuilder<CompanyEntity> builder)
    {
        builder.Metadata.SetSchema("Student");
        builder.ToTable("companies");
        builder.HasIndex(c => new { c.CompanyName, c.Designation });

        builder.Property(c => c.CompanyName)
                                        .HasColumnType("varchar(50)")
                                        .IsRequired();

        builder.Property(c => c.Designation)
                                        .HasColumnType("varchar(30)")
                                        .IsRequired();

        builder.Property(c => c.YearOfJoining)
                                        .HasColumnType("SMALLINT")
                                        .IsRequired();
        builder.HasOne(c => c.Student)
            .WithMany(s => s.Companies)
            .HasForeignKey(c => c.StudentId)
            .IsRequired();
    }
}
