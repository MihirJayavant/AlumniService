namespace Alumni.Faculty;

public sealed record GetAllFaculties : PaginationInput
{

}

public sealed class GetAllFacultyValidator : AbstractValidator<GetAllFaculties>
{
    public GetAllFacultyValidator() => this.ApplyPaginationRules();
}

public class GetAllFacultiesHandler(IFacultyDbContext context)
    : IHandler<GetAllFaculties, PaginatedList<FacultyResponse>>
{
    public AbstractValidator<GetAllFaculties> Validator { get; } = new GetAllFacultyValidator();

    public async Task<OneOf<PaginatedList<FacultyResponse>, ErrorType>> Handle(GetAllFaculties request,
        CancellationToken cancellationToken)
    {
        var result = await context.Faculties
            .OrderBy(f => f.Id)
            .Paginate(request.PageNumber, request.PageSize, cancellationToken);
        return result.WithItems(f => f.ToFacultyResponse());
    }
}
