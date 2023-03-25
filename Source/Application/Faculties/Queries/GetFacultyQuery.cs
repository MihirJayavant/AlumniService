namespace Application.Faculties;

public sealed record GetFacultyQuery : IRequest<OneOf<FacultyResponse, ErrorType>>
{
    public string Email { get; }

    public GetFacultyQuery(string email) => Email = email;
}

public sealed class GetFacultyQueryValidator : AbstractValidator<GetFacultyQuery>
{
    public GetFacultyQueryValidator() => RuleFor(x => x.Email).EmailAddress();
}

public class GetFacultyHandler : IRequestHandler<GetFacultyQuery, OneOf<FacultyResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public GetFacultyHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public Task<OneOf<FacultyResponse, ErrorType>> Handle(GetFacultyQuery request, CancellationToken cancellationToken)
    {
        return ValidationHelper.ValidateAndRun(request, new GetFacultyQueryValidator(), GetData);

        async Task<OneOf<FacultyResponse, ErrorType>> GetData()
        {
            var result = await context.Faculties
                                    .Where(f => f.Email == request.Email)
                                    .ProjectTo<FacultyResponse>(mapper.ConfigurationProvider)
                                    .FirstOrDefaultAsync(cancellationToken);
            if (result is null)
            {
                return new ErrorType(ResponseStatus.NotFound, "Student not found");
            }
            return result;
        }
    }

}
