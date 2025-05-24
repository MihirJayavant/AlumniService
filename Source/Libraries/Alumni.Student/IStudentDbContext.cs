using Alumni.Student.Companies;

namespace Alumni.Student;

public interface IStudentDbContext
{
    public DbSet<Student> Students { get; }
    public DbSet<Company> Companies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
