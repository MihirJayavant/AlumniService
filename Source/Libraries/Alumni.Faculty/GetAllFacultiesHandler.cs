namespace Alumni.Faculty;

public sealed record GetAllFaculties
{
    public required PaginationInput Pagination { get; init; }

}

public sealed class GetAllFacultyValidator : AbstractValidator<GetAllFaculties>
{
    public GetAllFacultyValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

public class GetAllFacultiesHandler(IFacultyDbContext context)
    : IHandler<GetAllFaculties, PaginatedList<FacultyResponse>>
{
    public AbstractValidator<GetAllFaculties> Validator { get; } = new GetAllFacultyValidator();

    public async Task<OneOf<PaginatedList<FacultyResponse>, ErrorType>> Handle(GetAllFaculties request,
        CancellationToken cancellationToken)
    {
        var result = await context.Faculties
            .Paginate(request.Pagination.PageNumber, request.Pagination.PageSize, cancellationToken);
        return result.WithItems(f => f.ToFacultyResponse());
    }
}
