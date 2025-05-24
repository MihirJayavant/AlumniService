namespace Alumni.Student.Exam;

public sealed record GetExam
{
    public int StudentId { get; init; }
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
        var data = await context.Exams.Where(s => s.StudentId == request.StudentId)
            .Paginate(1, 10, cancellationToken);
        return new PaginatedList<ExamResponse>()
        {
            Items = data.Items.Select(e => e.ToExamResponse()).ToList(),
            TotalCount = data.TotalCount,
            PageNumber = data.PageNumber,
            PageSize = data.PageSize,
        };
    }
}
