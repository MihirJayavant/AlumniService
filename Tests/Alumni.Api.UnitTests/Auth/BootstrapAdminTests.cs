using Alumni.Auth.Bootstrap;
using Core;
using OneOf;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class BootstrapAdminTests
{
    [Fact]
    public async Task Execute_WhenValid_ForwardsRequestAndCancellationToStore()
    {
        var store = new BootstrapStoreStub();
        var handler = new BootstrapAdminHandler(store);
        var request = new BootstrapAdmin("admin@example.com", "ValidPassword123!");

        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.Equal("admin-account", result.AsT0.UserId);
        Assert.Equal("admin@example.com", store.Request?.Email);
        Assert.Equal(request.Password, store.Request?.Password);
        Assert.Equal(TestContext.Current.CancellationToken, store.CancellationToken);
        Assert.Equal(1, store.Calls);
    }

    [Theory]
    [InlineData(null, "ValidPassword123!")]
    [InlineData("", "ValidPassword123!")]
    [InlineData("not-an-email", "ValidPassword123!")]
    [InlineData("admin@example.com", null)]
    [InlineData("admin@example.com", "")]
    [InlineData("admin@example.com", "short")]
    public async Task Execute_WhenCredentialsAreInvalid_RejectsWithoutStore(string? email, string? password)
    {
        var store = new BootstrapStoreStub();

        var result = await new BootstrapAdminHandler(store).Execute(
            new BootstrapAdmin(email!, password!), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task Execute_WhenAdminExists_PreservesConflict()
    {
        var error = new ErrorType { Status = ResponseStatus.Conflict, Message = "Bootstrap is unavailable." };
        var store = new BootstrapStoreStub { Result = error };

        var result = await new BootstrapAdminHandler(store).Execute(
            new BootstrapAdmin("admin@example.com", "ValidPassword123!"), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Same(error, result.AsT1);
    }

    [Fact]
    public async Task Execute_WhenEmailHasPadding_NormalizesEmailBeforeStore()
    {
        var store = new BootstrapStoreStub();

        var result = await new BootstrapAdminHandler(store).Execute(
            new BootstrapAdmin("  ADMIN@Example.COM  ", "ValidPassword123!"), TestContext.Current.CancellationToken);

        Assert.True(result.IsT0);
        Assert.Equal(new Email("ADMIN@Example.COM").Value, store.Request?.Email);
    }

    [Fact]
    public async Task Execute_WhenPasswordExceedsMaximum_RejectsWithoutStore()
    {
        var store = new BootstrapStoreStub();

        var result = await new BootstrapAdminHandler(store).Execute(
            new BootstrapAdmin("admin@example.com", new string('A', 257)), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task Execute_WhenStoreCancels_PropagatesCancellation()
    {
        var store = new BootstrapStoreStub { Cancel = true };

        var exception = await Assert.ThrowsAsync<OperationCanceledException>(() =>
            new BootstrapAdminHandler(store).Execute(
                new BootstrapAdmin("admin@example.com", "ValidPassword123!"), TestContext.Current.CancellationToken));

        Assert.Equal(TestContext.Current.CancellationToken, exception.CancellationToken);
    }

    [Fact]
    public void BootstrapAdmin_WhenFormatted_DoesNotExposePassword()
    {
        var request = new BootstrapAdmin("admin@example.com", "secret-new-password");

        Assert.DoesNotContain(request.Password, request.ToString(), StringComparison.Ordinal);
    }
}

internal sealed class BootstrapStoreStub : IAdminBootstrapStore
{
    public OneOf<AdminBootstrapped, ErrorType> Result { get; init; } = new AdminBootstrapped("admin-account", "admin@example.com");
    public bool Cancel { get; init; }
    public Exception? Failure { get; init; }
    public BootstrapAdmin? Request { get; private set; }
    public CancellationToken CancellationToken { get; private set; }
    public int Calls { get; private set; }

    public Task<OneOf<AdminBootstrapped, ErrorType>> CreateAsync(BootstrapAdmin request,
        CancellationToken cancellationToken = default)
    {
        Calls++;
        Request = request;
        CancellationToken = cancellationToken;
        if (Cancel)
        {
            throw new OperationCanceledException(cancellationToken);
        }

        if (Failure is not null)
        {
            throw Failure;
        }

        return Task.FromResult(Result);
    }
}
