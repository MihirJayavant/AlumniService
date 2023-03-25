namespace Application.Faculties;

public sealed record DeleteFacultyCommand : IRequest<OneOf<bool, ErrorType>>
{
    public int Id { get; }
    public DeleteFacultyCommand(int id) => Id = id;
}

public sealed class DeleteFacultyCommandValidator : AbstractValidator<DeleteFacultyCommand>
{
    public DeleteFacultyCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class DeleteFacultyHandler : IRequestHandler<DeleteFacultyCommand, OneOf<bool, ErrorType>>
{
    private readonly IApplicationDbContext context;

    public DeleteFacultyHandler(IApplicationDbContext context)
                    => this.context = context;

    public Task<OneOf<bool, ErrorType>> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
    {
        return ValidationHelper.ValidateAndRun(request, new DeleteFacultyCommandValidator(), GetData);

        async Task<OneOf<bool, ErrorType>> GetData()
        {
            var faculty = await context.Faculties
                        .FindAsync(new object[] { request.Id }, cancellationToken: cancellationToken);

            if (faculty is null)
            {
                return new ErrorType(ResponseStatus.NotFound, "Not Found");
            }
            context.Faculties.Remove(faculty);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
