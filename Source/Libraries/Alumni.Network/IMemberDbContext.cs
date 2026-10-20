using Alumni.Network.Companies;
using Alumni.Network.Exams;
using Alumni.Network.FurtherStudies;

namespace Alumni.Network;

public interface IMemberDbContext
{
    public DbSet<MemberEntity> Members { get; }
    public DbSet<CompanyEntity> Companies { get; }
    public DbSet<ExamEntity> Exams { get; }
    public DbSet<FurtherStudyEntity> FurtherStudies { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
