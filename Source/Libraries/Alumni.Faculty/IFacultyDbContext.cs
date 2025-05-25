namespace Alumni.Faculty;

public interface IFacultyDbContext
{
    public DbSet<Faculty> Faculties { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
