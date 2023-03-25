namespace Application.Exams;

public sealed record AddExamCommand : IRequest<OneOf<ExamResponse, ErrorType>>
{
    public required string ExamName { get; init; }
    public required int Score { get; init; }
    public required int Year { get; init; }
    public required int StudentId { get; init; }
}

public sealed class AddExamCommandValidator : AbstractValidator<AddExamCommand>
{
    public AddExamCommandValidator()
        => RuleFor(x => x.StudentId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id should be at least greater than or equal to 1.");
}

public class AddExamHandler : IRequestHandler<AddExamCommand, OneOf<ExamResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public AddExamHandler(IApplicationDbContext context, IMapper mapper)
                => (this.context, this.mapper) = (context, mapper);

    public Task<OneOf<ExamResponse, ErrorType>> Handle(AddExamCommand request, CancellationToken cancellationToken)
    {
        return ValidationHelper.ValidateAndRun(request, new AddExamCommandValidator(), GetData);

        async Task<OneOf<ExamResponse, ErrorType>> GetData()
        {
            var student = await context.Students
                            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

            if (student is null)
            {
                return new ErrorType(ResponseStatus.NotFound, "Student not found");
            }

            var exam = mapper.Map<Exam>(request);
            context.Exams.Add(exam);
            await context.SaveChangesAsync(cancellationToken);
            var response = mapper.Map<ExamResponse>(exam);

            return response;
        }
    }

}
