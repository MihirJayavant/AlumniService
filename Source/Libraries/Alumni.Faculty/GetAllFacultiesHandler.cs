namespace Alumni.Faculty;

public sealed record GetAllFaculties : PaginationInput
{

}

public sealed class GetAllFacultyValidator : AbstractValidator<GetAllFaculties>
{
    public GetAllFacultyValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
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
            .Paginate(request.PageNumber, request.PageSize, cancellationToken);
        return result.WithItems(f => f.ToFacultyResponse());
    }
}
