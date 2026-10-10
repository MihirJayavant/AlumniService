using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Alumni.Auth;

public sealed record BootstrapAdmin(string Email, string Password)
{
    public override string ToString() => $"BootstrapAdmin {{ Email = {Email}, Password = [REDACTED] }}";
}

public sealed record AdminBootstrapped(string UserId, string Email);

file sealed class BootstrapAdminValidator : AbstractValidator<BootstrapAdmin>
{
    public BootstrapAdminValidator()
    {
        RuleFor(request => request.Email).ValidEmail(maximumLength: 254);
        RuleFor(request => request.Password).Cascade(CascadeMode.Stop).NotEmpty().MinimumLength(8).MaximumLength(256);
    }
}

public sealed partial class BootstrapAdminHandler(
        IAuthDbContext context,
        UserManager<AuthUser> users,
        TimeProvider timeProvider,
        ILogger<BootstrapAdminHandler> logger
    ) : IHandler<BootstrapAdmin, AdminBootstrapped>
{
    public AbstractValidator<BootstrapAdmin> Validator { get; } = new BootstrapAdminValidator();

    public async Task<OneOf<AdminBootstrapped, ErrorType>> Handle(BootstrapAdmin request, CancellationToken cancellationToken)
    {
        try
        {
            return await BootstrapAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // Provider exceptions can contain account or credential values.
            if (logger.IsEnabled(LogLevel.Error))
            {
                LogBootstrapFailure(logger, exception.GetType().Name);
            }
            return new ErrorType { Message = "Administrator bootstrap could not be completed." };
        }
    }

    private async Task<OneOf<AdminBootstrapped, ErrorType>> BootstrapAsync(BootstrapAdmin request, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            LogBootstrapStarted(logger, request.Email);
        }
        var accountExists = await context.Users.AnyAsync(cancellationToken);
        if (accountExists)
        {
            LogAccountExists(logger);
            return new ErrorType
            {
                Message = "An account already exists.",
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

        if (created is { Succeeded: false, Errors: var errors })
        {
            var passwordErrors = errors.Where(error => error.Code.StartsWith("Password", StringComparison.Ordinal)).ToArray();
            return new ErrorType
            {
                Message = passwordErrors.Length > 0
                    ? string.Join(", ", passwordErrors.Select(error => error.Description))
                    : "Administrator bootstrap could not be completed.",
                Status = ResponseStatus.BadRequest
            };
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            LogUserCreated(logger, account.Email, account.Id);
            LogRoleAssignmentStarted(logger, account.Id);
        }

        var assigned = await users.AddToRoleAsync(account, AuthRoles.FacultyAdmin);
        if (!assigned.Succeeded)
        {
            return new ErrorType
            {
                Message = "Administrator bootstrap could not be completed.",
                Status = ResponseStatus.InternalError
            };
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            LogRoleAssigned(logger, account.Id);
        }

        return new AdminBootstrapped(account.Id, account.Email);
    }

    [LoggerMessage(EventId = 1002, Level = LogLevel.Error, Message = "Administrator bootstrap failed ({ExceptionType}).")]
    private static partial void LogBootstrapFailure(ILogger logger, string exceptionType);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Information, Message = "Bootstrapping administrator account for {Email}")]
    private static partial void LogBootstrapStarted(ILogger logger, string email);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Warning, Message = "Administrator bootstrap failed: an account already exists.")]
    private static partial void LogAccountExists(ILogger logger);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Information, Message = "Administrator user created for {Email} with UserId {UserId}")]
    private static partial void LogUserCreated(ILogger logger, string? email, string userId);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Information, Message = "Assigning administrator role to user {UserId}")]
    private static partial void LogRoleAssignmentStarted(ILogger logger, string userId);

    [LoggerMessage(EventId = 1007, Level = LogLevel.Information, Message = "Administrator role assigned to user {UserId}")]
    private static partial void LogRoleAssigned(ILogger logger, string userId);
}
