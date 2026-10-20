namespace Alumni.Network.Exams;

[RecordView(typeof(Exam), nameof(Exam.Id))]
public sealed partial record AddExam
{
    public required Guid MemberId { get; init; }
}

file sealed class AddExamValidator : AbstractValidator<AddExam>
{
    public AddExamValidator()
    {
        RuleFor(x => x.MemberId).ValidGuid();
        RuleFor(x => x.ExamName).RequiredText(100);
        RuleFor(x => x.Score).InclusiveBetween(0, short.MaxValue);
        RuleFor(x => x.Year).ValidYear();
    }
}

public class AddExamHandler(IMemberDbContext context) : IHandler<AddExam, ExamResponse>
{
    public AbstractValidator<AddExam> Validator { get; } = new AddExamValidator();

    public async Task<OneOf<ExamResponse, ErrorType>> Handle(AddExam request, CancellationToken cancellationToken = default)
    {
        var member = await context.Members
            .FirstOrDefaultAsync(s => s.MemberId == request.MemberId, cancellationToken);

        if (member is null)
        {
            return new ErrorType { Message = "Member not found", Status = ResponseStatus.NotFound };
        }

        var exam = request.ToExam();
        member.Exams.Add(exam);
        await context.SaveChangesAsync(cancellationToken);

        return exam.ToExamResponse();
    }
}

public static class AddExamMapper
{
    public static ExamEntity ToExam(this AddExam request)
        => new ExamEntity
        {
            Id = 0,
            ExamId = Guid.CreateVersion7(),
            ExamName = request.ExamName.Trim(),
            Score = request.Score,
            Year = request.Year,
        };
}
