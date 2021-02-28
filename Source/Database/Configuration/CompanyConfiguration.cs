using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configuration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasIndex(c => new {c.CompanyName, c.Designation});

            builder.Property(c => c.CompanyName)
                                            .HasColumnType("varchar(50)")
                                            .IsRequired();

            builder.Property(c => c.Designation)
                                            .HasColumnType("varchar(30)")
                                            .IsRequired();

            builder.Property(c => c.YearOfJoining)
                                            .HasColumnType("SMALLINT")
                                            .IsRequired();

        }
    }
}
