using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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

public sealed class BootstrapAdminHandler(
        IAuthDbContext context,
        UserManager<AuthUser> users,
        TimeProvider timeProvider,
        ILogger<BootstrapAdminHandler> logger
    ) : IHandler<BootstrapAdmin, AdminBootstrapped>
{
    public AbstractValidator<BootstrapAdmin> Validator { get; } = new BootstrapAdminValidator();

    public async Task<OneOf<AdminBootstrapped, ErrorType>> Handle(BootstrapAdmin request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Bootstrapping administrator account for {Email}", request.Email);
        var adminRoleName = users.NormalizeName(AuthRoles.FacultyAdmin);
        var adminRoleId = await context.Roles.Where(role => role.NormalizedName == adminRoleName)
                .Select(role => role.Id).SingleAsync(cancellationToken);

        var adminExists = await context.Users.AnyAsync(cancellationToken);
        if (adminExists)
        {
            logger.LogWarning("Administrator bootstrap failed: an administrator account already exists.");
            return new ErrorType
            {
                Message = "An administrator account already exists.",
                Status = ResponseStatus.Conflict
            };
        }

        var normalizedEmail = users.NormalizeEmail(request.Email);
        var account = new AuthUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = true,
            Status = AccountStatus.Active,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        var created = await users.CreateAsync(account, request.Password);

        logger.LogInformation("Administrator user created for {Email} with UserId {UserId}", account.Email, account.Id);

        if (created is { Succeeded: false, Errors: var errors })
        {
            return new ErrorType
            {
                Message = string.Join(", ", errors.Select(e => e.Description)),
                Status = ResponseStatus.BadRequest
            };
        }

        logger.LogInformation("Assigning administrator role to user {UserId}", account.Id);

        var assigned = await users.AddToRoleAsync(account, AuthRoles.FacultyAdmin);
        if (assigned is { Succeeded: false, Errors: var assignErrors })
        {
            return new ErrorType
            {
                Message = string.Join(", ", assignErrors.Select(e => e.Description)),
                Status = ResponseStatus.InternalError
            };
        }

        logger.LogInformation("Administrator role assigned to user {UserId}", account.Id);

        return new AdminBootstrapped(account.Id, account.Email);
    }
}
