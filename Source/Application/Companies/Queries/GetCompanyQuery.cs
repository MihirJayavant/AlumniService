namespace Application.Companies;

public sealed record GetCompanyQuery : IRequest<OneOf<PaginatedList<CompanyResponse>, ErrorType>>
{
    public int StudentId { get; }

    public GetCompanyQuery(int studentId) => StudentId = studentId;
}

public sealed class GetCompanyQueryValidator : AbstractValidator<GetCompanyQuery>
{
    public GetCompanyQueryValidator()
        => RuleFor(x => x.StudentId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id should be at least greater than or equal to 1.");
}

public sealed class GetCompanyHandler : IRequestHandler<GetCompanyQuery, OneOf<PaginatedList<CompanyResponse>, ErrorType>>
{

    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public GetCompanyHandler(IApplicationDbContext context, IMapper mapper)
                => (this.context, this.mapper) = (context, mapper);

    public Task<OneOf<PaginatedList<CompanyResponse>, ErrorType>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        return ValidationHelper.ValidateAndRun(request, new GetCompanyQueryValidator(), GetData);

        async Task<OneOf<PaginatedList<CompanyResponse>, ErrorType>> GetData()
            => await context.Companies.Where(s => s.StudentId == request.StudentId)
                                    .ProjectTo<CompanyResponse>(mapper.ConfigurationProvider)
                                    .PaginatedListAsync(1, 50, cancellationToken);
    }
}
