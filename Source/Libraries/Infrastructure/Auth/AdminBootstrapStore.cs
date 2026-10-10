using Alumni.Auth;
using Alumni.Auth.Bootstrap;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Infrastructure.Auth;

public sealed class AdminBootstrapStore(
    ApplicationContext context,
    UserManager<AuthUser> users,
    TimeProvider timeProvider,
    ILogger<AdminBootstrapStore> logger) : IAdminBootstrapStore
{
    private static readonly Action<ILogger, string, Exception?> LogOperationFailure =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1002, "AdminBootstrapFailed"),
            "Administrator bootstrap failed ({ExceptionType}).");

    public async Task<OneOf<AdminBootstrapped, ErrorType>> CreateAsync(BootstrapAdmin request, CancellationToken cancellationToken = default)
    {
        // This operation owns the scoped unit of work; do not invoke amid unrelated pending writes.
        try
        {
            var validation = await new BootstrapAdminHandler(this).Validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                context.ChangeTracker.Clear();
                return new ErrorType { Message = validation.Errors[0].ErrorMessage, Status = ResponseStatus.BadRequest };
            }

            var email = new Email(request.Email).Value;
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            // Shared with invitation operations: pending, disabled and active admins all close bootstrap.
            await context.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(71422024001)", cancellationToken);
            var adminRoleName = users.NormalizeName(AuthRoles.FacultyAdmin);
            var adminRoleId = await context.Roles.Where(role => role.NormalizedName == adminRoleName)
                .Select(role => role.Id).SingleAsync(cancellationToken);
            if (await context.UserRoles.AnyAsync(link => link.RoleId == adminRoleId, cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
                return Conflict("An administrator account already exists.");
            }

            var normalizedEmail = users.NormalizeEmail(email);
            if (await context.Users.AnyAsync(existingAccount => existingAccount.NormalizedEmail == normalizedEmail, cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
                return Conflict("An account with this email already exists.");
            }

            var now = timeProvider.GetUtcNow();
            var account = new AuthUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false,
                Status = AccountStatus.PendingActivation,
                CreatedAt = now
            };
            // Identity hashes and validates the supplied password. All intermediate saves remain atomic.
            var created = await users.CreateAsync(account, request.Password);
            if (!created.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
                return IdentityError(created);
            }

            var assigned = await users.AddToRoleAsync(account, AuthRoles.FacultyAdmin);
            if (!assigned.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
                return Failure();
            }

            account.Status = AccountStatus.Active;
            account.UpdatedAt = now;
            var activated = await users.UpdateAsync(account);
            if (!activated.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
                return Failure();
            }

            await transaction.CommitAsync(cancellationToken);
            var result = new AdminBootstrapped(account.Id, email);
            context.ChangeTracker.Clear();
            return result;
        }
        catch (DbUpdateException)
        {
            context.ChangeTracker.Clear();
            return Conflict("Administrator bootstrap conflicted with another account change.");
        }
        catch (OperationCanceledException)
        {
            context.ChangeTracker.Clear();
            throw;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            context.ChangeTracker.Clear();
            LogOperationFailure(logger, exception.GetType().Name, null);
            return Failure();
        }
    }

    private static ErrorType IdentityError(IdentityResult result)
    {
        var passwordErrors = result.Errors.Where(error => error.Code.StartsWith("Password", StringComparison.Ordinal)).ToList();
        return passwordErrors.Count > 0
            ? new ErrorType { Message = string.Join(" ", passwordErrors.Select(error => error.Description)), Status = ResponseStatus.BadRequest }
            : Failure();
    }

    private static ErrorType Failure() => new() { Message = "Administrator bootstrap could not be completed." };
    private static ErrorType Conflict(string message) => new() { Message = message, Status = ResponseStatus.Conflict };
}
