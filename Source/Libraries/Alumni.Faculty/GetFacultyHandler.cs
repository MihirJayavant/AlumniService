namespace Alumni.Faculty;

public sealed record GetFaculty
{
    public Guid FacultyId { get; init; }
}

file sealed class GetFacultyValidator : AbstractValidator<GetFaculty>
{

}

public class GetFacultyHandler(IFacultyDbContext context) : IHandler<GetFaculty, FacultyResponse>
{
    public AbstractValidator<GetFaculty> Validator { get; } = new GetFacultyValidator();

    public async Task<OneOf<FacultyResponse, ErrorType>> Handle(GetFaculty request, CancellationToken cancellationToken)
    {
        var result = await context.Faculties
                                .Where(f => f.FacultyId == request.FacultyId)
                                .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            return new ErrorType { Status = ResponseStatus.NotFound, Message = "Faculty not found" };
        }
        return result.ToFacultyResponse();
    }
}
