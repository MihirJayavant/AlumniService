namespace Alumni.Student.Exam;

[RecordView(typeof(Exam), nameof(Exam.Id))]
public sealed partial record AddExam
{
    public required Guid StudentId { get; init; }
}

file sealed class AddExamValidator : AbstractValidator<AddExam>
{

}

public class AddExamHandler(IStudentDbContext context) : IHandler<AddExam, ExamResponse>
{
    public AbstractValidator<AddExam> Validator { get; } = new AddExamValidator();

    public async Task<OneOf<ExamResponse, ErrorType>> Handle(AddExam request, CancellationToken cancellationToken = default)
    {
        var student = await context.Students
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (student is null)
        {
            return new ErrorType { Message = "Student not found", Status = ResponseStatus.NotFound };
        }

        var exam = new ExamEntity()
        {
            Id = 0,
            ExamId = Guid.CreateVersion7(),
            ExamName = request.ExamName,
            Score = request.Score,
            Year = request.Year,
        };
        student.Exams.Add(exam);
        await context.SaveChangesAsync(cancellationToken);

        return exam.ToExamResponse();
    }
}
