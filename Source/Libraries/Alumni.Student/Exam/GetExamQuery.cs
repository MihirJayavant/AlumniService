namespace Alumni.Student.Exam;

public sealed record GetExam
{
    public Guid StudentId { get; init; }
}

file sealed class GetExamValidator : AbstractValidator<GetExam>
{

}

public class GetExamHandler(IStudentDbContext context) : IHandler<GetExam, PaginatedList<ExamResponse>>
{
    public AbstractValidator<GetExam> Validator { get; } = new GetExamValidator();

    public async Task<OneOf<PaginatedList<ExamResponse>, ErrorType>> Handle(GetExam request,
        CancellationToken cancellationToken = default)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);
        if (student is null)
        {
            return new ErrorType { Message = "Student not found", Status = ResponseStatus.NotFound };
        }

        var result = await context.Exams.Where(s => s.StudentId == student.Id)
            .Paginate(1, 10, cancellationToken);
        return result.WithItems(e => e.ToExamResponse());
    }
}
