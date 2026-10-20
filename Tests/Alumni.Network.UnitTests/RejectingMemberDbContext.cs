using Alumni.Network.Companies;
using Alumni.Network.Exams;
using Alumni.Network.FurtherStudies;
using Microsoft.EntityFrameworkCore;

namespace Alumni.Network.UnitTests;

internal sealed class RejectingMemberDbContext : IMemberDbContext
{
    public int QueryAccessCount { get; private set; }
    public int SaveChangesCount { get; private set; }

    public DbSet<MemberEntity> Members => RejectAccess<MemberEntity>();
    public DbSet<CompanyEntity> Companies => RejectAccess<CompanyEntity>();
    public DbSet<ExamEntity> Exams => RejectAccess<ExamEntity>();
    public DbSet<FurtherStudyEntity> FurtherStudies => RejectAccess<FurtherStudyEntity>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        throw new InvalidOperationException("Unit validation must not save changes.");
    }

    private DbSet<T> RejectAccess<T>() where T : class
    {
        QueryAccessCount++;
        throw new InvalidOperationException("Unit validation must not access the database.");
    }
}
