namespace Alumni.Staff;

public interface IStaffDbContext
{
    public DbSet<StaffMemberEntity> StaffMembers { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
