namespace Application.Exams;

public sealed record GetExamQuery : IRequest<OneOf<PaginatedList<ExamResponse>, ErrorType>>
{
    public int StudentId { get; }

    public GetExamQuery(int studentId) => StudentId = studentId;
}

public sealed class GetExamQueryValidator : AbstractValidator<GetExamQuery>
{
    public GetExamQueryValidator()
        => RuleFor(x => x.StudentId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id should be at least greater than or equal to 1.");
}

public class GetExamHandler : IRequestHandler<GetExamQuery, OneOf<PaginatedList<ExamResponse>, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public GetExamHandler(IApplicationDbContext context, IMapper mapper)
                => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<PaginatedList<ExamResponse>, ErrorType>> Handle(GetExamQuery request, CancellationToken cancellationToken)
            => await context.Exams.Where(s => s.StudentId == request.StudentId)
                             .ProjectTo<ExamResponse>(mapper.ConfigurationProvider)
                             .PaginatedListAsync(1, 50, cancellationToken);


}
