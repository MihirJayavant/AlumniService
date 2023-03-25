using FluentValidation;
using OneOf;

namespace Application.Common.Models;

public class ValidationHelper
{

    public static async Task<OneOf<TResponse, ErrorType>> ValidateAndRun<TRequest, TResponse>
    (
        TRequest request,
        AbstractValidator<TRequest> validator,
        Func<Task<OneOf<TResponse, ErrorType>>> handler
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.IsValid is false)
        {
            var err = validationResult.Errors.First().ErrorMessage;
            return new ErrorType(ResponseStatus.BadRequest, err!);
        }
        return await handler();
    }
}
