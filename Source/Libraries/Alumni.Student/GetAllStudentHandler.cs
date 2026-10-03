namespace Alumni.Student;

public sealed record GetAllStudent : PaginationInput
{

}

public sealed class GetAllStudentValidator : AbstractValidator<GetAllStudent>
{
    public GetAllStudentValidator() => this.ApplyPaginationRules();
}

public sealed class GetAllStudentHandler(IStudentDbContext context)
    : IHandler<GetAllStudent, PaginatedList<StudentResponse>>
{
    public AbstractValidator<GetAllStudent> Validator { get; } = new GetAllStudentValidator();

    public async Task<OneOf<PaginatedList<StudentResponse>, ErrorType>> Handle(GetAllStudent request,
        CancellationToken cancellationToken = default)
    {
        var result = await context.Students
            .OrderBy(s => s.Id)
            .Paginate(request.PageNumber, request.PageSize, cancellationToken);

        return result.WithItems(s => s.ToStudentResponse());
    }
}
