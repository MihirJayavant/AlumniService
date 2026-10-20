namespace Alumni.Network.Companies;

[RecordView(typeof(Company), nameof(Company.Id))]
public sealed partial record AddCompany
{
    public required Guid MemberId { get; init; }
}

public class AddCompanyValidator : AbstractValidator<AddCompany>
{
    public AddCompanyValidator()
    {
        RuleFor(x => x.MemberId).ValidGuid();
        RuleFor(c => c.CompanyName).RequiredText(50);
        RuleFor(c => c.Designation).RequiredText(30);
        RuleFor(c => c.YearOfJoining).ValidYear();
        RuleFor(c => c.AnnualSalary).GreaterThanOrEqualTo(0);
    }
}

public class AddCompanyHandler(IMemberDbContext context) : IHandler<AddCompany, CompanyResponse>
{
    public AbstractValidator<AddCompany> Validator { get; } = new AddCompanyValidator();
    public async Task<OneOf<CompanyResponse, ErrorType>> Handle(AddCompany request, CancellationToken cancellationToken = default)
    {
        var member = await context.Members
                            .FirstOrDefaultAsync(s => s.MemberId == request.MemberId, cancellationToken);

        if (member is null)
        {
            return new ErrorType
            {
                Message = "Member not found",
                Status = ResponseStatus.NotFound
            };
        }

        var company = request.ToCompany(member);
        context.Companies.Add(company);
        await context.SaveChangesAsync(cancellationToken);
        var result = company.ToCompanyResponse();

        return result;
    }
}

public static class AddCompanyMapper
{
    public static CompanyEntity ToCompany(this AddCompany request, MemberEntity member)
        => new CompanyEntity
        {
            Id = 0,
            CompanyId = Guid.CreateVersion7(),
            CompanyName = request.CompanyName.Trim(),
            Designation = request.Designation.Trim(),
            YearOfJoining = request.YearOfJoining,
            AnnualSalary = request.AnnualSalary,
            MemberId = member.Id,
            Member = member,
        };
}
