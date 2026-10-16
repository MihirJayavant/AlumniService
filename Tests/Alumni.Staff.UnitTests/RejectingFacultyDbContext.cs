using Microsoft.EntityFrameworkCore;

namespace Alumni.Staff.UnitTests;

internal sealed class RejectingFacultyDbContext : IStaffDbContext
{
    public int QueryAccessCount { get; private set; }
    public int SaveChangesCount { get; private set; }

    public DbSet<StaffMemberEntity> StaffMembers
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
