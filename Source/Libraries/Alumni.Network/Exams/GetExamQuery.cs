namespace Alumni.Network.Exams;

public sealed record GetExam
{
    public Guid MemberId { get; init; }
}

file sealed class GetExamValidator : AbstractValidator<GetExam>
{
    public GetExamValidator() => RuleFor(x => x.MemberId).ValidGuid();
}

public class GetExamHandler(IMemberDbContext context) : IHandler<GetExam, PaginatedList<ExamResponse>>
{
    public AbstractValidator<GetExam> Validator { get; } = new GetExamValidator();

    public async Task<OneOf<PaginatedList<ExamResponse>, ErrorType>> Handle(GetExam request,
        CancellationToken cancellationToken = default)
    {
        var member = await context.Members.FirstOrDefaultAsync(s => s.MemberId == request.MemberId, cancellationToken);
        if (member is null)
        {
            return new ErrorType { Message = "Member not found", Status = ResponseStatus.NotFound };
        }

        var result = await context.Exams.Where(s => s.MemberId == member.Id)
            .Paginate(1, 10, cancellationToken);
        return result.WithItems(e => e.ToExamResponse());
    }
}
