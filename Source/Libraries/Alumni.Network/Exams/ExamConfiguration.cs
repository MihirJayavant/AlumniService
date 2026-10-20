using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Network.Exams;

public class ExamConfiguration : IEntityTypeConfiguration<ExamEntity>
{
    public void Configure(EntityTypeBuilder<ExamEntity> builder)
    {
        builder.ToTable("exams", "Network");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.ExamId).IsRequired();
        builder.HasIndex(e => e.ExamId).IsUnique();

        builder.HasIndex(e => e.ExamName);

        builder.Property(e => e.ExamName)
                                    .HasColumnType("varchar(100)")
                                    .IsRequired();

        builder.Property(e => e.Score)
                                    .HasColumnType("SMALLINT")
                                    .IsRequired();

        builder.Property(e => e.Year)
                                    .HasColumnType("SMALLINT")
                                    .IsRequired();

        builder.HasOne(e => e.Member)
            .WithMany(s => s.Exams)
            .HasForeignKey(e => e.MemberId)
            .IsRequired();

    }
}
