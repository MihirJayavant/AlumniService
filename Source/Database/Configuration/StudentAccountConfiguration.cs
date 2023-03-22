namespace Database.Configuration;

public class StudentAccountConfiguration : IEntityTypeConfiguration<StudentAccount>
{
    public void Configure(EntityTypeBuilder<StudentAccount> builder)
    {
        builder.Property(s => s.Email).HasColumnType("varchar(50)")
                                                    .IsRequired();

        builder.Property(s => s.Password).HasColumnType("varchar(200)")
                                                    .IsRequired();

    }
}
