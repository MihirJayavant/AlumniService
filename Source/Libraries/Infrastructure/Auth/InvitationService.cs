using System.Security.Cryptography;
using System.Text;
using Alumni.Auth;
using Alumni.Auth.Invitations;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Infrastructure.Auth;

public sealed class InvitationService(
    ApplicationContext context,
    UserManager<AuthUser> users,
    TimeProvider timeProvider,
    ILogger<InvitationService> logger) : IInvitationService, IInvitationProvisioningService
{
    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromHours(48);
    private static readonly Action<ILogger, string, Exception?> LogOperationFailure =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1001, "InvitationOperationFailed"),
            "Invitation operation failed ({ExceptionType}).");

    public Task<OneOf<InvitationIssued, ErrorType>> InviteStudentAsync(string invitedByUserId, AddStudent profile, CancellationToken cancellationToken = default) =>
        InTransaction<InvitationIssued>(async () =>
        {
            if (!await IsAdmin(invitedByUserId, cancellationToken))
            {
                return Forbidden();
            }

            var validation = await new AddStudentHandler(context).Validator.ValidateAsync(profile, cancellationToken);
            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors[0].ErrorMessage);
            }

            var email = new Email(profile.Email).Value;
            if (await EmailExists(email, cancellationToken))
            {
                return Conflict("An account or profile with this email already exists.");
            }

            var account = NewAccount(email);
            var error = await CreateAccount(account, AuthRoles.Student);
            if (error is not null)
            {
                return error;
            }

            var student = profile.ToStudent();
            student.AuthUserId = account.Id;
            context.Students.Add(student);
            return await Issue(account, AuthRoles.Student, invitedByUserId, cancellationToken);
        }, cancellationToken);

    public Task<OneOf<InvitationIssued, ErrorType>> InviteFacultyAsync(string invitedByUserId, AddFaculty profile, string role, CancellationToken cancellationToken = default) =>
        InTransaction<InvitationIssued>(async () =>
        {
            var validation = await new AddFacultyHandler(context).Validator.ValidateAsync(profile, cancellationToken);
            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors[0].ErrorMessage);
            }

            var email = new Email(profile.Email).Value;
            if (!await IsAdmin(invitedByUserId, cancellationToken))
            {
                return Forbidden();
            }

            if (role is not (AuthRoles.FacultyReader or AuthRoles.FacultyEditor or AuthRoles.FacultyAdmin))
            {
                return BadRequest("Select a valid faculty role.");
            }

            if (await EmailExists(email, cancellationToken))
            {
                return Conflict("An account or profile with this email already exists.");
            }

            var account = NewAccount(email);
            var error = await CreateAccount(account, role);
            if (error is not null)
            {
                return error;
            }

            var now = timeProvider.GetUtcNow().UtcDateTime;
            context.Faculties.Add(new Faculty
            {
                Id = 0,
                FacultyId = Guid.CreateVersion7(),
                AuthUserId = account.Id,
                Email = email,
                FirstName = profile.FirstName.Trim(),
                LastName = profile.LastName.Trim(),
                Extension = profile.Extension.Trim(),
                MobileNo = profile.MobileNo,
                CreatedAt = now,
                UpdatedAt = now
            });
            return await Issue(account, role, invitedByUserId, cancellationToken);
        }, cancellationToken);

    public Task<OneOf<InvitationAccepted, ErrorType>> AcceptAsync(AcceptInvitation request, CancellationToken cancellationToken = default) =>
        InTransaction<InvitationAccepted>(async () =>
        {
            if (!InvitationToken.TryHash(request.Token, out var hash))
            {
                return InvalidInvitation();
            }

            var invitation = await context.Invitations.SingleOrDefaultAsync(value => value.TokenHash == hash, cancellationToken);
            var now = timeProvider.GetUtcNow();
            if (invitation is null || invitation.AcceptedAt is not null || invitation.RevokedAt is not null || invitation.ExpiresAt <= now)
            {
                return InvalidInvitation();
            }

            var account = await context.Users.SingleOrDefaultAsync(value => value.Id == invitation.UserId, cancellationToken);
            if (!CanActivate(account, invitation))
            {
                return InvalidInvitation();
            }

            var assignedRoles = await context.UserRoles.Where(link => link.UserId == account!.Id).Select(link => link.RoleId).ToListAsync(cancellationToken);
            if (assignedRoles.Count != 1 || assignedRoles[0] != invitation.RoleId)
            {
                return InvalidInvitation();
            }

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length > 256)
            {
                return BadRequest("Supply a password of at most 256 characters.");
            }

            var passwordResult = await users.AddPasswordAsync(account!, request.Password);
            if (!passwordResult.Succeeded)
            {
                return IdentityError(passwordResult);
            }

            account!.Status = AccountStatus.Active;
            account.EmailConfirmed = true;
            account.UpdatedAt = now;
            invitation.AcceptedAt = now;
            var update = await users.UpdateAsync(account);
            if (!update.Succeeded)
            {
                return InvalidInvitation();
            }

            await context.SaveChangesAsync(cancellationToken);
            return new InvitationAccepted(account.Id);
        }, cancellationToken, acceptance: true);

    public Task<OneOf<InvitationIssued, ErrorType>> ResendAsync(string invitedByUserId, Guid invitationId, CancellationToken cancellationToken = default) =>
        InTransaction<InvitationIssued>(async () =>
        {
            if (!await IsAdmin(invitedByUserId, cancellationToken))
            {
                return Forbidden();
            }

            var invitation = await context.Invitations.SingleOrDefaultAsync(value => value.Id == invitationId, cancellationToken);
            if (invitation is null || invitation.AcceptedAt is not null)
            {
                return InvalidInvitation();
            }

            var account = await context.Users.SingleOrDefaultAsync(value => value.Id == invitation.UserId, cancellationToken);
            if (!CanActivate(account, invitation))
            {
                return InvalidInvitation();
            }

            var role = await context.Roles.Where(value => value.Id == invitation.RoleId).Select(value => value.Name).SingleAsync(cancellationToken);
            var assignedRoles = await users.GetRolesAsync(account!);
            if (role is null || assignedRoles.Count != 1 || assignedRoles[0] != role)
            {
                return InvalidInvitation();
            }

            await RevokeOutstanding(account!.Id, cancellationToken);
            return await Issue(account, role, invitedByUserId, cancellationToken);
        }, cancellationToken);

    public Task<OneOf<InvitationRevoked, ErrorType>> RevokeAsync(string invitedByUserId, Guid invitationId, CancellationToken cancellationToken = default) =>
        InTransaction<InvitationRevoked>(async () =>
        {
            if (!await IsAdmin(invitedByUserId, cancellationToken))
            {
                return Forbidden();
            }

            var invitation = await context.Invitations.SingleOrDefaultAsync(value => value.Id == invitationId, cancellationToken);
            if (invitation is null || invitation.AcceptedAt is not null)
            {
                return InvalidInvitation();
            }

            // Revoke the account's outstanding links too, including any replacement link.
            await RevokeOutstanding(invitation.UserId, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new InvitationRevoked(invitationId);
        }, cancellationToken);

    private bool CanActivate(AuthUser? account, Invitation invitation) =>
        account is { Status: AccountStatus.PendingActivation, PasswordHash: null }
        && account.NormalizedEmail == invitation.NormalizedEmail
        && users.NormalizeEmail(account.Email!) == invitation.NormalizedEmail;

    private async Task RevokeOutstanding(string userId, CancellationToken cancellationToken)
    {
        var outstanding = await context.Invitations.Where(value => value.UserId == userId && value.AcceptedAt == null && value.RevokedAt == null).ToListAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        foreach (var invitation in outstanding)
        {
            invitation.RevokedAt = now;
        }
    }

    private AuthUser NewAccount(string email) => new()
    {
        UserName = email,
        Email = email,
        Status = AccountStatus.PendingActivation,
        CreatedAt = timeProvider.GetUtcNow()
    };

    private async Task<ErrorType?> CreateAccount(AuthUser account, string role)
    {
        var created = await users.CreateAsync(account);
        if (!created.Succeeded)
        {
            return IdentityError(created);
        }
        var assigned = await users.AddToRoleAsync(account, role);
        return assigned.Succeeded ? null : IdentityError(assigned);
    }

    private async Task<InvitationIssued> Issue(AuthUser account, string role, string invitedByUserId, CancellationToken cancellationToken)
    {
        var token = InvitationToken.Create();
        var now = timeProvider.GetUtcNow();
        var invitation = new Invitation
        {
            Id = Guid.CreateVersion7(),
            UserId = account.Id,
            Email = account.Email!,
            NormalizedEmail = users.NormalizeEmail(account.Email!)!,
            RoleId = await RoleId(role, cancellationToken),
            InvitedByUserId = invitedByUserId,
            TokenHash = InvitationToken.Hash(token),
            CreatedAt = now,
            ExpiresAt = now + InvitationLifetime
        };
        context.Invitations.Add(invitation);
        await context.SaveChangesAsync(cancellationToken);
        return new InvitationIssued(invitation.Id, account.Id, invitation.Email, role, token, invitation.ExpiresAt);
    }

    private async Task<string> RoleId(string role, CancellationToken cancellationToken)
    {
        var normalizedRole = users.NormalizeName(role);
        return await context.Roles.Where(value => value.NormalizedName == normalizedRole).Select(value => value.Id).SingleAsync(cancellationToken);
    }

    private async Task<bool> IsAdmin(string userId, CancellationToken cancellationToken)
    {
        var roleId = await RoleId(AuthRoles.FacultyAdmin, cancellationToken);
        return await (from account in context.Users
                      join role in context.UserRoles on account.Id equals role.UserId
                      where account.Id == userId && account.Status == AccountStatus.Active
                          && account.PasswordHash != null && role.RoleId == roleId
                      select account.Id).AnyAsync(cancellationToken);
    }

    private async Task<bool> EmailExists(string email, CancellationToken cancellationToken)
    {
        var normalized = users.NormalizeEmail(email);
        var pattern = email.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal).Replace("_", "\\_", StringComparison.Ordinal);
        return await context.Users.AnyAsync(value => value.NormalizedEmail == normalized, cancellationToken)
            || await context.Students.AnyAsync(value => EF.Functions.ILike(value.Email, pattern, "\\"), cancellationToken)
            || await context.Faculties.AnyAsync(value => EF.Functions.ILike(value.Email, pattern, "\\"), cancellationToken);
    }

    private async Task<OneOf<T, ErrorType>> InTransaction<T>(Func<Task<OneOf<T, ErrorType>>> operation, CancellationToken cancellationToken, bool acceptance = false)
    {
        // This service owns the scoped unit of work. Do not invoke it amid unrelated pending writes.
        // A rollback must also discard tracked mutations so a later call cannot flush aborted activation.
        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            // One shared transaction lock serializes bootstrap, issuance, acceptance and revocation.
            // Future account administration must use this same lock before changing account roles/status.
            await context.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(71422024001)", cancellationToken);
            var result = await operation();
            if (result.IsT0)
            {
                await transaction.CommitAsync(cancellationToken);
                // Subsequent operations in this scope must reload account and invitation state.
                context.ChangeTracker.Clear();
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
                context.ChangeTracker.Clear();
            }
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            context.ChangeTracker.Clear();
            return acceptance ? InvalidInvitation() : Conflict("The account or invitation changed. Try again.");
        }
        catch (DbUpdateException)
        {
            context.ChangeTracker.Clear();
            return acceptance ? InvalidInvitation() : Conflict("The invitation could not be saved. The account or profile may already exist.");
        }
        catch (OperationCanceledException)
        {
            context.ChangeTracker.Clear();
            throw;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            context.ChangeTracker.Clear();
            // Do not log exception details: providers can include account/token/password values.
            LogOperationFailure(logger, exception.GetType().Name, null);
            return new ErrorType { Message = "The invitation operation could not be completed." };
        }
    }

    private static ErrorType IdentityError(IdentityResult result) =>
        result.Errors.Any(error => error.Code == "ConcurrencyFailure")
            ? InvalidInvitation()
            : BadRequest(string.Join(" ", result.Errors.Select(error => error.Description)));

    private static ErrorType InvalidInvitation() => BadRequest("The invitation is invalid or no longer available.");
    private static ErrorType BadRequest(string message) => new() { Message = message, Status = ResponseStatus.BadRequest };
    private static ErrorType Conflict(string message) => new() { Message = message, Status = ResponseStatus.Conflict };
    private static ErrorType Forbidden() => new() { Message = "An active faculty administrator account is required.", Status = ResponseStatus.Forbidden };
}

public static class InvitationToken
{
    public static string Create() => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

    public static bool TryHash(string? token, out string hash)
    {
        hash = string.Empty;
        if (token is null || token.Length != 43 || token.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not ('-' or '_')))
        {
            return false;
        }
        hash = Hash(token);
        return true;
    }

    public static string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
