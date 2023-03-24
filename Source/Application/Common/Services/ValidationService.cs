using FluentValidation;

namespace Apllication.Common.Services;

public interface IValidationService
{
    Response<T> CreateErrorResponse<T>(string error, ResponseStatus status = ResponseStatus.BadRequest);
    Response<T> CreateSuccessResponse<T>(T data, ResponseStatus status = ResponseStatus.Success);
    string Validate<T>(T data, AbstractValidator<T> validator);
    public Task<Response<TResponse>> Compose<TRequest, TResponse>
    (
        TRequest request,
        AbstractValidator<TRequest> validator,
        Func<Task<Response<TResponse>>> handler
    );
}

public class ValidationService : IValidationService
{
    public string Validate<T>(T request, AbstractValidator<T> validator)
        => validator.Validate(request).ToString();


    public Response<T> CreateErrorResponse<T>(string error, ResponseStatus status = ResponseStatus.BadRequest)
        => new Response<T>
        {
            Error = new ErrorResponse(error),
            Status = status
        };

    public Response<T> CreateSuccessResponse<T>(T data, ResponseStatus status = ResponseStatus.Success)
        => new()
        {
            Status = status,
            Result = data
        };

    public async Task<Response<TResponse>> Compose<TRequest, TResponse>
    (
        TRequest request,
        AbstractValidator<TRequest> validator,
        Func<Task<Response<TResponse>>> handler
    )
    {
        var validationResult = Validate(request, validator);
        if (validationResult != string.Empty)
        {
            return CreateErrorResponse<TResponse>(validationResult);
        }

        return await handler();
    }


}
