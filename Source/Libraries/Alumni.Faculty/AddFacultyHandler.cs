namespace Alumni.Faculty;

[RecordView(typeof(Faculty), nameof(Faculty.AuthUserId), nameof(Faculty.Id), nameof(Faculty.FacultyId), nameof(Faculty.IsDeleted), nameof(Faculty.CreatedAt), nameof(Faculty.UpdatedAt))]
public sealed partial record AddFaculty
{

}

file sealed class AddFacultyValidator : AbstractValidator<AddFaculty>
{
    public AddFacultyValidator()
    {
        RuleFor(x => x.Email).ValidEmail(maximumLength: 100);

        RuleFor(x => x.FirstName).RequiredText(100).WithMessage("FirstName must be at most 100 characters.");
        RuleFor(x => x.LastName).RequiredText(100).WithMessage("LastName must be at most 100 characters.");
        RuleFor(x => x.Extension).RequiredText(10);

        RuleFor(x => x.MobileNo).GreaterThan(0);
    }
}

public class AddFacultyHandler(IFacultyDbContext context) : IHandler<AddFaculty, FacultyResponse>
{
    public AbstractValidator<AddFaculty> Validator { get; } = new AddFacultyValidator();

    public async Task<OneOf<FacultyResponse, ErrorType>> Handle(AddFaculty request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email).Value;
        var found = await context.Faculties
                    .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);

        if (found is not null)
        {
            return new ErrorType
            {
                Message = "Faculty with this email already exists",
                Status = ResponseStatus.Conflict
            };
        }

        var createdAt = DateTime.UtcNow;
        var faculty = new Faculty()
        {
            Id = 0,
            FacultyId = Guid.NewGuid(),
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Extension = request.Extension.Trim(),
            MobileNo = request.MobileNo,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
        context.Faculties.Add(faculty);

        await context.SaveChangesAsync(cancellationToken);

        return faculty.ToFacultyResponse();
    }
}
