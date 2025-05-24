using Alumni.Student.Company;
using Alumni.Student.Exam;
using Alumni.Student.FurtherStudy;

namespace Alumni.Student;

public record StudentEntity : Student
{
    public IList<CompanyEntity> Companies { get; init; } = [];
    public IList<ExamEntity> Exams { get; init; } = [];
    public IList<FurtherStudyEntity> FurtherStudies { get; init; } = [];
}
