namespace Application.FurtherStudies;

public class GetFurtherStudyQuery : IRequest<OneOf<PaginatedList<FurtherStudyResponse>, ErrorType>>
{
    public int StudentId { get; }

    public GetFurtherStudyQuery(int studentId) => StudentId = studentId;
}

public sealed class GetFurtherStudyQueryValidator : AbstractValidator<GetFurtherStudyQuery>
{
    public GetFurtherStudyQueryValidator()
        => RuleFor(x => x.StudentId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id should be at least greater than or equal to 1.");
}

public class GetFurtherStudyHandler : IRequestHandler<GetFurtherStudyQuery, OneOf<PaginatedList<FurtherStudyResponse>, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public GetFurtherStudyHandler(IApplicationDbContext context, IMapper mapper)
                        => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<PaginatedList<FurtherStudyResponse>, ErrorType>> Handle(GetFurtherStudyQuery request, CancellationToken cancellationToken)
                         => await context.FurtherStudies
                                    .Where(s => s.StudentId == request.StudentId)
                                    .ProjectTo<FurtherStudyResponse>(mapper.ConfigurationProvider)
                                    .PaginatedListAsync(1, 50, cancellationToken);
}
