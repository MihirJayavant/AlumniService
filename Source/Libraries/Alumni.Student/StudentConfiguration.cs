using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alumni.Student;

public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
{
    public void Configure(EntityTypeBuilder<StudentEntity> builder)
    {
        builder.Metadata.SetSchema("Student");
        builder.ToTable("students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.StudentId);
        builder.HasIndex(s => s.StudentId).IsUnique();

        builder.Property(s => s.Email).HasColumnType("varchar(100)")
                                        .IsRequired();
        builder.HasIndex(s => s.Email).IsUnique();

        builder.Property(s => s.FirstName).HasColumnType("varchar(100)")
                                        .IsRequired();

        builder.Property(s => s.LastName).HasColumnType("varchar(100)")
                                        .IsRequired();


        builder.Property(s => s.Gender)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(s => s.Branch)
            .IsRequired()
            .HasColumnType("varchar(30)");

        builder.Property(s => s.Extension)
                        .HasColumnType("varchar(10)")
                        .IsRequired();

        builder.Property(s => s.MobileNo)
                        .IsRequired();

        builder.Property(s => s.DateOfBirth)
                        .IsRequired();

        builder.Property(s => s.CreatedAt)
                        .IsRequired();

        builder.Property(s => s.UpdatedAt)
                        .IsRequired();


        builder.Property(s => s.AdmissionYear)
                        .HasColumnType("SMALLINT")
                        .IsRequired();
        builder.Property(s => s.PassingYear)
                        .HasColumnType("SMALLINT")
                        .IsRequired();

        builder.OwnsOne(
            s => s.CurrentAddress,
            addressBuilder =>
            {
                addressBuilder.Property(a => a.Country)
                                .HasColumnType("varchar(100)")
                                .IsRequired();
                addressBuilder.Property(a => a.State)
                                .HasColumnType("varchar(100)")
                                .IsRequired();
                addressBuilder.Property(a => a.City)
                                .HasColumnType("varchar(100)")
                                .IsRequired();
                addressBuilder.Property(a => a.UserAddress)
                                .HasColumnType("varchar(100)")
                                .IsRequired();
                addressBuilder.Property(a => a.PinCode)
                                .IsRequired();
            }
        );


        builder.OwnsOne(
            s => s.CorrespondenceAddress,
            addressBuilder =>
            {
                addressBuilder.Property(a => a.Country)
                                .HasColumnType("varchar(30)")
                                .IsRequired();
                addressBuilder.Property(a => a.State)
                                .HasColumnType("varchar(30)")
                                .IsRequired();
                addressBuilder.Property(a => a.City)
                                .HasColumnType("varchar(30)")
                                .IsRequired();
                addressBuilder.Property(a => a.UserAddress)
                                .HasColumnType("varchar(100)")
                                .IsRequired();
                addressBuilder.Property(a => a.PinCode)
                                .IsRequired();
            }
        );

    }
}
