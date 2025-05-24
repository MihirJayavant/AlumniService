namespace Alumni.Student.Exam;

public record ExamEntity : Exam
{
    public int StudentId { get; set; }
    public StudentEntity Student { get; set; }
}
