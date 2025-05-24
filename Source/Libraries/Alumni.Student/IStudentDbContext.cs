using Alumni.Student.Exam;

namespace Alumni.Student;

public interface IStudentDbContext
{
    public DbSet<StudentEntity> Students { get; }
    public DbSet<Company.Company> Companies { get; }
    public DbSet<ExamEntity> Exams { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
