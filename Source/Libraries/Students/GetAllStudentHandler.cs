namespace Students;

public sealed record GetAllStudent
{
    public required PaginationQuery Pagination { get; init; }
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
        var data = await context.Students.Skip((request.Pagination.PageNumber -1 ) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);
        var totalCount = await context.Students.CountAsync(cancellationToken);
        var result = data.Select(s => s.ToStudentResponse()).ToList();

        return new PaginatedList<StudentResponse>()
        {
            Items = result,
            PageNumber = request.Pagination.PageNumber,
            PageSize = request.Pagination.PageSize,
            TotalCount = totalCount,
        };
    }
}
