using Alumni.Student.Company;
using Alumni.Student.Exam;

namespace Alumni.Student;

public record StudentEntity : Student
{
    public IList<CompanyEntity> Companies { get; init; } = [];
    public IList<ExamEntity> Exams { get; init; } = [];
}
