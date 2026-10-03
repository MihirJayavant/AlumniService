using Alumni.Student.Company;
using Alumni.Student.Exam;
using Alumni.Student.FurtherStudy;
using Microsoft.EntityFrameworkCore;

namespace Alumni.Student.UnitTests;

internal sealed class RejectingStudentDbContext : IStudentDbContext
{
    public int QueryAccessCount { get; private set; }
    public int SaveChangesCount { get; private set; }

    public DbSet<StudentEntity> Students => RejectAccess<StudentEntity>();
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
