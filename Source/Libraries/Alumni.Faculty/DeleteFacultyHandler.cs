namespace Alumni.Faculty;

public sealed record DeleteFaculty
{
    public Guid FacultyId { get; init; }
}

file sealed class DeleteFacultyValidator : AbstractValidator<DeleteFaculty>
{

}

public class DeleteFacultyHandler(IFacultyDbContext context)
    : IHandler<DeleteFaculty, FacultyResponse>
{
    public AbstractValidator<DeleteFaculty> Validator { get; } = new DeleteFacultyValidator();

    public async Task<OneOf<FacultyResponse, ErrorType>> Handle(DeleteFaculty request, CancellationToken cancellationToken)
    {
        var faculty = await context.Faculties.FirstOrDefaultAsync(f => f.FacultyId == request.FacultyId, cancellationToken);

        if (faculty is null)
        {
            return new ErrorType { Status = ResponseStatus.NotFound, Message = "Faculty not found" };
        }

        context.Faculties.Remove(faculty);
        await context.SaveChangesAsync(cancellationToken);
        return faculty.ToFacultyResponse();
    }
}
