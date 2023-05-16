namespace Application.Companies;

public sealed record AddCompanyCommand : IRequest<OneOf<CompanyResponse, ErrorType>>
{
    public required string CompanyName { get; init; }
    public required string Designation { get; init; }
    public required int YearOfJoining { get; init; }
    public required long AnnualSalary { get; init; }
    public required int StudentId { get; init; }
}

public class AddCompanyValidator : AbstractValidator<AddCompanyCommand>
{
    public AddCompanyValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty();
        RuleFor(c => c.Designation).NotEmpty();
    }
}

public class AddCompanyHandler : IRequestHandler<AddCompanyCommand, OneOf<CompanyResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public AddCompanyHandler(IApplicationDbContext context, IMapper mapper)
                => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<CompanyResponse, ErrorType>> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students
                            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (student is null)
        {
            return new ErrorType(ResponseStatus.NotFound, "Student not found");
        }

        var company = mapper.Map<Company>(request);
        context.Companies.Add(company);
        await context.SaveChangesAsync(cancellationToken);
        var result = mapper.Map<CompanyResponse>(company);

        return result;
    }
}
