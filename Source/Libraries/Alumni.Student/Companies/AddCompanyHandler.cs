namespace Alumni.Student.Companies;

[RecordView(typeof(Company), nameof(Company.Id), nameof(Company.Student), nameof(Company.StudentId))]
public sealed partial record AddCompany
{
    public required Guid StudentId { get; init; }
}

public class AddCompanyValidator : AbstractValidator<AddCompany>
{
    public AddCompanyValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty();
        RuleFor(c => c.Designation).NotEmpty();
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

        var company = new Company()
        {
            Id = 0,
            CompanyId = Guid.CreateVersion7(),
            CompanyName = request.CompanyName,
            Designation = request.Designation,
            YearOfJoining = request.YearOfJoining,
            AnnualSalary = request.AnnualSalary,
            StudentId = student.Id,
            Student = student,
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync(cancellationToken);
        var result = company.ToCompanyResponse();

        return result;
    }
}
