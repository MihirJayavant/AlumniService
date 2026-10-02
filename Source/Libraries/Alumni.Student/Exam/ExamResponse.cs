namespace Alumni.Student.Exam;

[RecordView(typeof(Exam), nameof(Exam.Id))]
public sealed partial record ExamResponse
{

}

public static class ExamResponseMapper
{
    public static ExamResponse ToExamResponse(this Exam exam) =>
        new()
        {
            ExamId = exam.ExamId,
            ExamName = exam.ExamName,
            Score = exam.Score,
            Year = exam.Year,
        };
}
