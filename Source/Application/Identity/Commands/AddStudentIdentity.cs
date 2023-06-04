namespace Application.Identity;

public sealed record AddStudentIdentityCommand : IRequest<OneOf<IdentityResponse, ErrorType>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public sealed class AddStudentIdentityCommandValidator : AbstractValidator<AddStudentIdentityCommand>
{
    public AddStudentIdentityCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8)
                                .MaximumLength(20);
    }
}

public class AddStudentIdentityHandler : IRequestHandler<AddStudentIdentityCommand, OneOf<IdentityResponse, ErrorType>>
{
    private readonly IIdentityService identityService;

    public AddStudentIdentityHandler(IIdentityService identityService) => this.identityService = identityService;

    public async Task<OneOf<IdentityResponse, ErrorType>> Handle(AddStudentIdentityCommand request, CancellationToken cancellationToken)
                => await identityService.RegisterStudent(request.Email, request.Password);
}
