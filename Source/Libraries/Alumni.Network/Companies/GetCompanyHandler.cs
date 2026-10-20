namespace Alumni.Network.Companies;

public sealed record GetCompany
{
    public required Guid MemberId { get; init; }
}

file sealed class GetCompanyValidator : AbstractValidator<GetCompany>
{
    public GetCompanyValidator() => RuleFor(x => x.MemberId).ValidGuid();
}

public sealed class GetCompanyHandler(IMemberDbContext context) : IHandler<GetCompany, PaginatedList<CompanyResponse>>
{

    public AbstractValidator<GetCompany> Validator { get; } = new GetCompanyValidator();

    public async Task<OneOf<PaginatedList<CompanyResponse>, ErrorType>> Handle(GetCompany request,
        CancellationToken cancellationToken = default)
    {
        var member = await context.Members.FirstOrDefaultAsync(s => s.MemberId == request.MemberId, cancellationToken);
        if (member is null)
        {
            return new ErrorType { Message = "Member not found", Status = ResponseStatus.NotFound };
        }

        var result = await context.Companies.Where(s => s.MemberId == member.Id)
            .Paginate(1, 10, cancellationToken);

        return result.WithItems(c => c.ToCompanyResponse());
    }
}
