namespace Alumni.Student;

[RecordView(typeof(Student))]
public sealed partial record AddStudent
{

}

file sealed class AddStudentValidator : AbstractValidator<AddStudent>
{
    public AddStudentValidator()
    {
        RuleFor(x => x.Email).ValidEmail(maximumLength: 100);
        RuleFor(x => x.FirstName).RequiredText(100);
        RuleFor(x => x.LastName).RequiredText(100);
        RuleFor(x => x.Extension).RequiredText(10);
        RuleFor(x => x.MobileNo).ValidMobileNumber();
        RuleFor(x => x.Gender).ValidGender();
        RuleFor(x => x.Branch).RequiredText(30);
        RuleFor(x => x.DateOfBirth).ValidDateOfBirth();
        RuleFor(x => x.AdmissionYear)
            .ValidYear()
            .GreaterThanOrEqualTo(x => x.DateOfBirth.Year)
            .WithMessage("AdmissionYear must not precede the birth year.");
        RuleFor(x => x.PassingYear)
            .ValidYear()
            .GreaterThanOrEqualTo(x => x.AdmissionYear)
            .WithMessage("PassingYear must not precede AdmissionYear.");
        RuleFor(x => x.CurrentAddress).NotNull().SetValidator(new AddressValidator(100));
        RuleFor(x => x.CorrespondenceAddress).NotNull().SetValidator(new AddressValidator(100));
    }
}


public class AddStudentHandler(IStudentDbContext context) : IHandler<AddStudent, StudentResponse>
{
    public AbstractValidator<AddStudent> Validator { get; } = new AddStudentValidator();

    public async Task<OneOf<StudentResponse, ErrorType>> Handle(AddStudent request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email).Value;
        var account = await context.Students
                    .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);

        if (account is not null)
        {
            return new ErrorType
            {
                Message = "Student with this email already exists",
                Status = ResponseStatus.Conflict
            };
        }

        var student = request.ToStudent();
        context.Students.Add(student);
        await context.SaveChangesAsync(cancellationToken);

        return student.ToStudentResponse();
    }
}


public static class AddStudentMapper
{
    public static StudentEntity ToStudent(this AddStudent student) =>
        new()
        {
            Id = 0,
            StudentId = Guid.CreateVersion7(),
            FirstName = student.FirstName.Trim(),
            LastName = student.LastName.Trim(),
            MobileNo = student.MobileNo.Trim(),
            Extension = student.Extension.Trim(),
            Gender = string.Equals(student.Gender.Trim(), Gender.Male, StringComparison.OrdinalIgnoreCase)
                ? Gender.Male : Gender.Female,
            DateOfBirth = student.DateOfBirth,
            Email = new Email(student.Email).Value,
            Branch = student.Branch.Trim(),
            CurrentAddress = NormalizeAddress(student.CurrentAddress),
            CorrespondenceAddress = NormalizeAddress(student.CorrespondenceAddress),
            AdmissionYear = student.AdmissionYear,
            PassingYear = student.PassingYear,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    private static Address NormalizeAddress(Address address) => address with
    {
        PinCode = address.PinCode.Trim(),
        Country = address.Country.Trim(),
        State = address.State.Trim(),
        City = address.City.Trim(),
        UserAddress = address.UserAddress.Trim()
    };
}
