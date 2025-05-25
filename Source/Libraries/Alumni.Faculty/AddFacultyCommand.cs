namespace Alumni.Faculty;

[RecordView(typeof(Faculty), nameof(Faculty.Id), nameof(Faculty.IsDeleted), nameof(Faculty.CreatedAt), nameof(Faculty.UpdatedAt))]
public sealed partial record AddFaculty
{

}

file sealed class AddFacultyValidator : AbstractValidator<AddFaculty>
{
    public AddFacultyValidator() => RuleFor(x => x.Email).EmailAddress();
}

public class AddFacultyHandler(IFacultyDbContext context) : IHandler<AddFaculty, FacultyResponse>
{
    public AbstractValidator<AddFaculty> Validator { get; } = new AddFacultyValidator();

    public async Task<OneOf<FacultyResponse, ErrorType>> Handle(AddFaculty request, CancellationToken cancellationToken)
    {
        var found = await context.Faculties
                    .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (found is not null)
        {
            return new ErrorType
            {
                Message = "Faculty with this email already exists", Status = ResponseStatus.Conflict
            };
        }

        var faculty = new Faculty()
        {
            Id = 0,
            FacultyId = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Extension = request.Extension,
            MobileNo = request.MobileNo
        };
        context.Faculties.Add(faculty);
        await context.SaveChangesAsync(cancellationToken);

        return faculty.ToFacultyResponse();
    }
}
