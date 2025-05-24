namespace Alumni.Student.Company;

public sealed record GetCompany
{
    public required Guid StudentId { get; init; }
}

file sealed class GetCompanyValidator : AbstractValidator<GetCompany>
{

}

public sealed class GetCompanyHandler(IStudentDbContext context) : IHandler<GetCompany, PaginatedList<CompanyResponse>>
{

    public AbstractValidator<GetCompany> Validator { get; } = new GetCompanyValidator();

    public async Task<OneOf<PaginatedList<CompanyResponse>, ErrorType>> Handle(GetCompany request,
        CancellationToken cancellationToken = default)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);
        if (student is null)
        {
            return new ErrorType { Message = "Student not found", Status = ResponseStatus.NotFound };
        }

        var result = await context.Companies.Where(s => s.StudentId == student.Id)
            .Paginate(1, 10, cancellationToken);

        return result.WithItems(c => c.ToCompanyResponse());
    }
}
