namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<StudentAccount> StudentAccount { get;}
    public DbSet<Student> Students { get;}
    public DbSet<Faculty> Faculties { get;}
    public DbSet<Company> Companies { get;}
    public DbSet<Exam> Exams { get;}
    public DbSet<FurtherStudy> FurtherStudies { get;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
