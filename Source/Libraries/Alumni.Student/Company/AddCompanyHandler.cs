namespace Alumni.Student.Company;

[RecordView(typeof(Company), nameof(Company.Id))]
public sealed partial record AddCompany
{
    public required Guid StudentId { get; init; }
}

public class AddCompanyValidator : AbstractValidator<AddCompany>
{
    public AddCompanyValidator()
    {
        RuleFor(x => x.StudentId).ValidGuid();
        RuleFor(c => c.CompanyName).RequiredText(50);
        RuleFor(c => c.Designation).RequiredText(30);
        RuleFor(c => c.YearOfJoining).ValidYear();
        RuleFor(c => c.AnnualSalary).GreaterThanOrEqualTo(0);
    }
}

public class AddCompanyHandler(IStudentDbContext context) : IHandler<AddCompany, CompanyResponse>
{
    public AbstractValidator<AddCompany> Validator { get; } = new AddCompanyValidator();
    public async Task<OneOf<CompanyResponse, ErrorType>> Handle(AddCompany request, CancellationToken cancellationToken = default)
    {
        var student = await context.Students
                            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (student is null)
        {
            return new ErrorType
            {
                Message = "Student not found",
                Status = ResponseStatus.NotFound
            };
        }

        var company = request.ToCompany(student);
        context.Companies.Add(company);
        await context.SaveChangesAsync(cancellationToken);
        var result = company.ToCompanyResponse();

        return result;
    }
}

public static class AddCompanyMapper
{
    public static CompanyEntity ToCompany(this AddCompany request, StudentEntity student)
        => new CompanyEntity
        {
            Id = 0,
            CompanyId = Guid.CreateVersion7(),
            CompanyName = request.CompanyName.Trim(),
            Designation = request.Designation.Trim(),
            YearOfJoining = request.YearOfJoining,
            AnnualSalary = request.AnnualSalary,
            StudentId = student.Id,
            Student = student,
        };
}
