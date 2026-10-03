using Microsoft.EntityFrameworkCore;

namespace Alumni.Faculty.UnitTests;

internal sealed class RejectingFacultyDbContext : IFacultyDbContext
{
    public int QueryAccessCount { get; private set; }
    public int SaveChangesCount { get; private set; }

    public DbSet<Faculty> Faculties
    {
        get
        {
            QueryAccessCount++;
            throw new InvalidOperationException("Unit validation must not access the database.");
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        throw new InvalidOperationException("Unit validation must not save changes.");
    }
}
