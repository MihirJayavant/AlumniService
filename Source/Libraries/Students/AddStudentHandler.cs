using Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Riok.Mapperly.Abstractions;

namespace Students;

public sealed record AddStudent
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
    public required string Gender { get; init; }
    public required string Email { get; init; }
    public required DateTime DateOfBirth { get; init; }

    public required string Branch { get; init; }
    public required Address CurrentAddress { get; init; }
    public required Address CorrespondenceAddress { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }

}

public sealed class AddStudentValidator : AbstractValidator<AddStudent>
{
    public AddStudentValidator() => RuleFor(x => x.Email).EmailAddress();
}


public class AddStudentHandler(IStudentDbContext context) : IHandler<AddStudent, Student>
{
    public AbstractValidator<AddStudent> Validator { get; } = new AddStudentValidator();

    public async Task<OneOf<Student, ErrorType>> Handle(AddStudent request, CancellationToken cancellationToken)
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

        var mapper = new AddStudentMapper();
        var student = mapper.ToCore(request);
        context.Students.Add(student);
        await context.SaveChangesAsync(cancellationToken);

        return student;
    }
}

[Mapper]
public partial class AddStudentMapper
{
    [MapValue(nameof(Student.Id), 0)]
    public partial Student ToCore(AddStudent student);
}
