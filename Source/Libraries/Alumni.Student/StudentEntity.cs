using Alumni.Student.Company;
using Alumni.Student.Exam;
using Alumni.Student.FurtherStudy;

namespace Alumni.Student;

[RecordView(typeof(Student))]
public partial record StudentEntity : IAuditableEntity
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public IList<CompanyEntity> Companies { get; init; } = [];
    public IList<ExamEntity> Exams { get; init; } = [];
    public IList<FurtherStudyEntity> FurtherStudies { get; init; } = [];
}
