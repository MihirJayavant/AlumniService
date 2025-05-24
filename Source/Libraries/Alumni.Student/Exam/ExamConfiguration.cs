using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Student.Exam;

public class ExamConfiguration : IEntityTypeConfiguration<ExamEntity>
{
    public void Configure(EntityTypeBuilder<ExamEntity> builder)
    {
        builder.Metadata.SetSchema("Student");
        builder.ToTable("exams");

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

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Exams)
            .HasForeignKey(e => e.StudentId)
            .IsRequired();

    }
}
