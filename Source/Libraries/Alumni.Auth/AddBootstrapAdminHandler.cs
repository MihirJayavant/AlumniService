namespace Alumni.Auth;

public sealed record BootstrapAdmin(string Email, string Password);

public sealed record AdminBootstrapped(string UserId, string Email);

file sealed class BootstrapAdminValidator : AbstractValidator<BootstrapAdmin>
{
    public BootstrapAdminValidator()
    {
        RuleFor(request => request.Email).ValidEmail(maximumLength: 254);
        RuleFor(request => request.Password).Cascade(CascadeMode.Stop).NotEmpty().MinimumLength(8).MaximumLength(256);
    }
}

public sealed class BootstrapAdminHandler() : IHandler<BootstrapAdmin, AdminBootstrapped>
{
    public AbstractValidator<BootstrapAdmin> Validator { get; } = new BootstrapAdminValidator();

    public Task<OneOf<AdminBootstrapped, ErrorType>> Handle(BootstrapAdmin request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
