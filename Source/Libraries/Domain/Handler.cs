using FluentValidation;
using OneOf;

namespace Domain;

public interface IHandler<TRequest, TResponse>
{
    public AbstractValidator<TRequest> Validator { get; }
    public Task<OneOf<TResponse, ErrorType>> Handle(TRequest request, CancellationToken cancellationToken = default);
}

public static class HandlerExtensions
{
    public static async Task<OneOf<TResponse, ErrorType>> Execute<TRequest, TResponse>(
        this IHandler<TRequest, TResponse> handler,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await handler.Validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ErrorType
                {
                    Message = validationResult.Errors.First().ErrorMessage,
                };
            }

            return await handler.Handle(request, cancellationToken);

        }
        catch (Exception ex)
        {
            return new ErrorType { Message = ex.Message };
        }
    }
}
