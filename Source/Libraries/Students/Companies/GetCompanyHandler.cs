namespace Students.Companies;

public sealed record GetCompany
{
    public required int StudentId { get; init; }
}

public sealed class GetCompanyValidator : AbstractValidator<GetCompany>
{

}

public sealed class GetCompanyHandler(IStudentDbContext context) : IHandler<GetCompany, PaginatedList<CompanyResponse>>
{

    public AbstractValidator<GetCompany> Validator { get; } = new GetCompanyValidator();

    public async Task<OneOf<PaginatedList<CompanyResponse>, ErrorType>> Handle(GetCompany request,
        CancellationToken cancellationToken = default)
    {
        var result = await context.Companies.Where(s => s.StudentId == request.StudentId)
            .Paginate(1, 10, cancellationToken);
        return new PaginatedList<CompanyResponse>()
        {
            Items = result.Items.Select(c => c.ToCompanyResponse()).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
        };
    }
}
