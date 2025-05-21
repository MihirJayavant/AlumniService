using Microsoft.EntityFrameworkCore;

namespace Students;

public interface IStudentDbContext
{
    public DbSet<Student> Students { get;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
