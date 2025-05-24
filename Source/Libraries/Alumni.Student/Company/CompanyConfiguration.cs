using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Student.Company;

public class CompanyConfiguration : IEntityTypeConfiguration<CompanyEntity>
{
    public void Configure(EntityTypeBuilder<CompanyEntity> builder)
    {
        builder.Metadata.SetSchema("Student");
        builder.ToTable("companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.CompanyId).IsRequired();
        builder.HasIndex(c => c.CompanyId).IsUnique();

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
