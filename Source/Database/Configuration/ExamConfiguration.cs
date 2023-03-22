namespace Database.Configuration;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.HasIndex(e => e.ExamName);

        builder.Property(e => e.ExamName)
                                    .HasColumnType("varchar(50)")
                                    .IsRequired();

        builder.Property(e => e.Score)
                                    .HasColumnType("SMALLINT")
                                    .IsRequired();

        builder.Property(e => e.Year)
                                    .HasColumnType("SMALLINT")
                                    .IsRequired();

    }
}
