using Application.Common.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Students.Commands;

public sealed record AddStudentCommand : IRequest<OneOf<StudentResponse, ErrorType>>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
    public required string Gender { get; init; }
    public required string Email { get; init; }
    public required DateTime DateOfBirth { get; init; }

    public required string Branch { get; init; }
    public required AddressResponse CurrentAddress { get; init; }
    public required AddressResponse CorrespondenceAddress { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }

}

public sealed class AddStudentCommandValidator : AbstractValidator<AddStudentCommand>
{
    public AddStudentCommandValidator() => RuleFor(x => x.Email).EmailAddress();
}


public class AddStudentHandler : IRequestHandler<AddStudentCommand, OneOf<StudentResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public AddStudentHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public Task<OneOf<StudentResponse, ErrorType>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
    {

        return ValidationHelper.ValidateAndRun(request, new AddStudentCommandValidator(), GetData);

        async Task<OneOf<StudentResponse, ErrorType>> GetData()
        {
            var account = await context.Students
                        .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

            if (account is not null)
            {
                return new ErrorType(ResponseStatus.Conflict, "Student email id already exist");
            }
            var student = mapper.Map<Student>(request);
            student.DateCreated = DateTime.Now;
            student.DateLastModified = DateTime.Now;
            var entity = context.Students.Add(student);
            await context.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<StudentResponse>(student);

            return result;

        }
    }

}
