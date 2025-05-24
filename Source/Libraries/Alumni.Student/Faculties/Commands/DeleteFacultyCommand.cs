namespace Application.Faculties;

public sealed record DeleteFacultyCommand : IRequest<OneOf<FacultyResponse, ErrorType>>
{
    public int Id { get; }
    public DeleteFacultyCommand(int id) => Id = id;
}

public sealed class DeleteFacultyCommandValidator : AbstractValidator<DeleteFacultyCommand>
{
    public DeleteFacultyCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public class DeleteFacultyHandler : IRequestHandler<DeleteFacultyCommand, OneOf<FacultyResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public DeleteFacultyHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<FacultyResponse, ErrorType>> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await context.Faculties
                    .FindAsync(new object[] { request.Id }, cancellationToken: cancellationToken);

        if (faculty is null)
        {
            return new ErrorType(ResponseStatus.NotFound, "Not Found");
        }

        context.Faculties.Remove(faculty);
        await context.SaveChangesAsync(cancellationToken);
        return mapper.Map<FacultyResponse>(faculty);
    }
}
