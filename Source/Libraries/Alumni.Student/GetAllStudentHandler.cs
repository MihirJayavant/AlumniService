namespace Alumni.Student;

public sealed record GetAllStudent
{
    public required PaginationInput Pagination { get; init; }
}

public sealed class GetAllStudentValidator : AbstractValidator<GetAllStudent>
{
    public GetAllStudentValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

public sealed class GetAllStudentHandler(IStudentDbContext context)
    : IHandler<GetAllStudent, PaginatedList<StudentResponse>>
{
    public AbstractValidator<GetAllStudent> Validator { get; } = new GetAllStudentValidator();

    public async Task<OneOf<PaginatedList<StudentResponse>, ErrorType>> Handle(GetAllStudent request,
        CancellationToken cancellationToken = default)
    {
        var result = await context.Students.Paginate(request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken);

        return result.WithItems(s=> s.ToStudentResponse());
    }
}
