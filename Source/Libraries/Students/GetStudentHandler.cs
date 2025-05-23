namespace Students;

public sealed record GetStudent
{
    public Guid Id { get; init; }
}

file sealed class GetStudentValidator : AbstractValidator<GetStudent>
{

}

public class GetStudentHandler(IStudentDbContext context) : IHandler<GetStudent, StudentResponse>
{
    public AbstractValidator<GetStudent> Validator { get; } = new GetStudentValidator();

    public async Task<OneOf<StudentResponse, ErrorType>> Handle(GetStudent request,
        CancellationToken cancellationToken)
    {
        var result = await context.Students
            .Where(s => s.StudentId == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return result switch
        {
            null => new ErrorType { Status = ResponseStatus.NotFound, Message = "Students not found" },
            _ => result.ToStudentResponse()
        };
    }
}
