using Students.Companies;

namespace Students;

public interface IStudentDbContext
{
    public DbSet<Student> Students { get;}
    public DbSet<Company> Companies { get;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
