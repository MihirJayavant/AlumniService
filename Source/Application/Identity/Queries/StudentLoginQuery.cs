namespace Application.Identity;

public sealed record StudentLoginQuery : IRequest<OneOf<IdentityResponse, ErrorType>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public sealed class StudentLoginQueryValidator : AbstractValidator<StudentLoginQuery>
{
    public StudentLoginQueryValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8)
                                .MaximumLength(20);
    }
}

public class StudentLoginHandler : IRequestHandler<StudentLoginQuery, OneOf<IdentityResponse, ErrorType>>
{
    private readonly IIdentityService identityService;

    public StudentLoginHandler(IIdentityService identityService) => this.identityService = identityService;

    public async Task<OneOf<IdentityResponse, ErrorType>> Handle(StudentLoginQuery request, CancellationToken cancellationToken)
                => await identityService.StudentLogin(request.Email, request.Password);
}
