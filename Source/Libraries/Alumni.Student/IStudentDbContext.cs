using Alumni.Student.Company;
using Alumni.Student.Exam;
using Alumni.Student.FurtherStudy;

namespace Alumni.Student;

public interface IStudentDbContext
{
    public DbSet<StudentEntity> Students { get; }
    public DbSet<CompanyEntity> Companies { get; }
    public DbSet<ExamEntity> Exams { get; }
    public DbSet<FurtherStudyEntity> FurtherStudies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
