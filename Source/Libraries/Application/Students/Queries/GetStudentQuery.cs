namespace Application.Students;

public sealed record GetStudentQuery : IRequest<OneOf<StudentResponse, ErrorType>>
{
    public string Email { get; }
    public GetStudentQuery(string email) => Email = email;
}

public sealed class GetStudentQueryValidator : AbstractValidator<GetStudentQuery>
{
    public GetStudentQueryValidator() => RuleFor(x => x.Email).EmailAddress();
}

public class GetStudentHandler : IRequestHandler<GetStudentQuery, OneOf<StudentResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public GetStudentHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<OneOf<StudentResponse, ErrorType>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var result = await context.Students
                                    .Where(s => s.Email == request.Email)
                                    .ProjectTo<StudentResponse>(mapper.ConfigurationProvider)
                                    .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            return new ErrorType(ResponseStatus.NotFound, "Students not found");
        }

        return result;
    }
}
