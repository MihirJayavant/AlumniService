namespace Students;

[RecordView(typeof(Student), nameof(Student.Id))]
public sealed partial record AddStudent
{

}

file sealed class AddStudentValidator : AbstractValidator<AddStudent>
{
    public AddStudentValidator() => RuleFor(x => x.Email).EmailAddress();
}


public class AddStudentHandler(IStudentDbContext context) : IHandler<AddStudent, StudentResponse>
{
    public AbstractValidator<AddStudent> Validator { get; } = new AddStudentValidator();

    public async Task<OneOf<StudentResponse, ErrorType>> Handle(AddStudent request, CancellationToken cancellationToken)
    {
        var account = await context.Students
                    .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

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
    public static Student ToStudent(this AddStudent student) =>
        new()
        {
            Id = 0,
            Uuid = Guid.CreateVersion7(),
            FirstName = student.FirstName,
            LastName = student.LastName,
            MobileNo = student.MobileNo,
            Extension = student.Extension,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            Email = student.Email,
            Branch = student.Branch,
            CurrentAddress = student.CurrentAddress,
            CorrespondenceAddress = student.CorrespondenceAddress,
            AdmissionYear = student.AdmissionYear,
            PassingYear = student.PassingYear
        };
}
