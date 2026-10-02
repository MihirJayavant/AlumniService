using FluentValidation;
using OneOf;
using Xunit;

namespace Core.UnitTests;

public sealed class HandlerExtensionsTests
{
    [Fact]
    public async Task Execute_WhenRequestIsValid_ReturnsResponseAndForwardsRequestAndToken()
    {
        using var cancellation = new CancellationTokenSource();
        var request = new TestRequest("Alumni");
        var response = new TestResponse("Saved");
        CancellationToken? validationToken = null;
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Name).MustAsync((_, token) =>
        {
            validationToken = token;
            return Task.FromResult(true);
        });
        var handler = new TestHandler(validator, (input, token) =>
        {
            Assert.Same(request, input);
            Assert.Equal(cancellation.Token, token);
            return Task.FromResult<OneOf<TestResponse, ErrorType>>(response);
        });

        var result = await handler.Execute(request, cancellation.Token);

        Assert.True(result.IsT0);
        Assert.Same(response, result.AsT0);
        Assert.Equal(cancellation.Token, validationToken);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task Execute_WhenValidationFails_ReturnsFirstErrorAndDoesNotInvokeHandler()
    {
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        validator.RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name is too short");
        var handler = new TestHandler(validator);

        var result = await handler.Execute(new TestRequest(string.Empty), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal("Name is required", result.AsT1.Message);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(0, handler.CallCount);
    }

    [Fact]
    public async Task Execute_WhenHandlerReturnsError_PreservesError()
    {
        var error = new ErrorType { Message = "Already exists", Status = ResponseStatus.Conflict };
        var handler = new TestHandler(new InlineValidator<TestRequest>(),
            (_, _) => Task.FromResult<OneOf<TestResponse, ErrorType>>(error));

        var result = await handler.Execute(new TestRequest("Alumni"), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Same(error, result.AsT1);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task Execute_WhenValidatorThrows_ReturnsInternalErrorAndDoesNotInvokeHandler()
    {
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Name).MustAsync((_, _) =>
            Task.FromException<bool>(new InvalidOperationException("Validation failed")));
        var handler = new TestHandler(validator);

        var result = await handler.Execute(new TestRequest("Alumni"), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal("Validation failed", result.AsT1.Message);
        Assert.Equal(ResponseStatus.InternalError, result.AsT1.Status);
        Assert.Equal(0, handler.CallCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Execute_WhenHandlerThrows_ReturnsInternalError(bool asynchronously)
    {
        var handler = new TestHandler(new InlineValidator<TestRequest>(), (_, _) =>
        {
            var exception = new InvalidOperationException("Save failed");
            if (asynchronously)
            {
                return Task.FromException<OneOf<TestResponse, ErrorType>>(exception);
            }

            throw exception;
        });

        var result = await handler.Execute(new TestRequest("Alumni"), TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal("Save failed", result.AsT1.Message);
        Assert.Equal(ResponseStatus.InternalError, result.AsT1.Status);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task Execute_WhenTokenIsAlreadyCanceled_DoesNotInvokeHandler()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Name).NotEmpty();
        var handler = new TestHandler(validator);

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            handler.Execute(new TestRequest("Alumni"), cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(0, handler.CallCount);
    }

    [Fact]
    public async Task Execute_WhenValidationIsCanceled_PropagatesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Name).MustAsync((_, token) =>
        {
            cancellation.Cancel();
            return Task.FromCanceled<bool>(token);
        });
        var handler = new TestHandler(validator);

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            handler.Execute(new TestRequest("Alumni"), cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(0, handler.CallCount);
    }

    [Fact]
    public async Task Execute_WhenHandlerIsCanceled_PropagatesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var handler = new TestHandler(new InlineValidator<TestRequest>(), (_, token) =>
        {
            cancellation.Cancel();
            return Task.FromCanceled<OneOf<TestResponse, ErrorType>>(token);
        });

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            handler.Execute(new TestRequest("Alumni"), cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Equal(1, handler.CallCount);
    }

    private sealed record TestRequest(string Name);

    private sealed record TestResponse(string Message);

    private sealed class TestHandler(
        AbstractValidator<TestRequest> validator,
        Func<TestRequest, CancellationToken, Task<OneOf<TestResponse, ErrorType>>>? handle = null)
        : IHandler<TestRequest, TestResponse>
    {
        public AbstractValidator<TestRequest> Validator { get; } = validator;

        public int CallCount { get; private set; }

        public Task<OneOf<TestResponse, ErrorType>> Handle(TestRequest request, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return handle?.Invoke(request, cancellationToken)
                ?? Task.FromResult<OneOf<TestResponse, ErrorType>>(new TestResponse("Saved"));
        }
    }
}
