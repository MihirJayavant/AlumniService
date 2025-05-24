namespace Application.Faculties;

public sealed record AddFacultyCommand : IRequest<OneOf<FacultyResponse, ErrorType>>
{
    public required string Email { get; init; }
    public required bool Admin { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required long MobileNo { get; init; }
    public required string Extension { get; init; }
}

public sealed class AddFacultyCommandValidator : AbstractValidator<AddFacultyCommand>
{
    public AddFacultyCommandValidator() => RuleFor(x => x.Email).EmailAddress();
}

public class AddFacultyHandler : IRequestHandler<AddFacultyCommand, OneOf<FacultyResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public AddFacultyHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<FacultyResponse, ErrorType>> Handle(AddFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await context.Faculties
                    .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (faculty is not null)
        {
            return new ErrorType(ResponseStatus.Conflict, "Email already registered");
        }

        var f = mapper.Map<Faculty>(request);
        context.Faculties.Add(f);
        await context.SaveChangesAsync(cancellationToken);

        var result = mapper.Map<FacultyResponse>(f);
        return result;
    }
}
