namespace Alumni.Student;

public interface IStudentDbContext
{
    public DbSet<Student> Students { get; }
    public DbSet<Company.Company> Companies { get; }
    public DbSet<Exam.Exam> Exams { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
