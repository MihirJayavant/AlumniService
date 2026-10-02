namespace Alumni.Faculty;

public interface IFacultyDbContext
{
    public DbSet<Faculty> Faculties { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
