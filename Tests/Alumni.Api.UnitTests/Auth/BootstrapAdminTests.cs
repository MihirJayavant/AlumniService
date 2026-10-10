using Alumni.Auth;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class BootstrapAdminTests
{
    [Theory]
    [InlineData(null, "ValidPassword123!")]
    [InlineData("", "ValidPassword123!")]
    [InlineData("not-an-email", "ValidPassword123!")]
    [InlineData("admin@example.com", null)]
    [InlineData("admin@example.com", "")]
    [InlineData("admin@example.com", "short")]
    public async Task Execute_WhenCredentialsAreInvalid_RejectsWithoutPersistence(string? email, string? password)
    {
        var context = new UnexpectedAuthDbContext();
        var handler = CreateHandler(context);

        var result = await handler.Execute(
            new BootstrapAdmin(email!, password!), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(0, context.Accesses);
    }

    [Theory]
    [InlineData(7, false)]
    [InlineData(8, true)]
    [InlineData(12, true)]
    [InlineData(256, true)]
    [InlineData(257, false)]
    public async Task Validator_WhenPasswordLengthIsAtBoundary_EnforcesEightTo256Characters(int length, bool valid)
    {
        var context = new UnexpectedAuthDbContext();
        var handler = CreateHandler(context);

        var result = await handler.Validator.ValidateAsync(
            new BootstrapAdmin("admin@example.com", new string('A', length)),
            TestContext.Current.CancellationToken);

        Assert.Equal(valid, result.IsValid);
        Assert.Equal(0, context.Accesses);
    }

    [Fact]
    public void BootstrapAdmin_WhenFormatted_DoesNotExposePassword()
    {
        var request = new BootstrapAdmin("admin@example.com", "secret-new-password");

        Assert.DoesNotContain(request.Password, request.ToString(), StringComparison.Ordinal);
    }

    internal static BootstrapAdminHandler CreateHandler(UnexpectedAuthDbContext context) =>
        new(context, null!, TimeProvider.System, NullLogger<BootstrapAdminHandler>.Instance);
}

internal sealed class UnexpectedAuthDbContext : IAuthDbContext
{
    public int Accesses { get; private set; }

    public DbSet<AuthUser> Users => UnexpectedAccess<DbSet<AuthUser>>();
    public DbSet<AuthRole> Roles => UnexpectedAccess<DbSet<AuthRole>>();
    public DbSet<IdentityUserRole<string>> UserRoles => UnexpectedAccess<DbSet<IdentityUserRole<string>>>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => UnexpectedAccess<Task<int>>();

    private T UnexpectedAccess<T>()
    {
        Accesses++;
        throw new InvalidOperationException("Validation must not access persistence.");
    }
}
