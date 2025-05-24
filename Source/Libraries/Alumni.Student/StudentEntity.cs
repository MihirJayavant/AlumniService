using Alumni.Student.Exam;

namespace Alumni.Student;

public record StudentEntity : Student
{
    public IList<Company.Company> Companies { get; init; } = [];
    public IList<ExamEntity> Exams { get; init; } = [];
}
