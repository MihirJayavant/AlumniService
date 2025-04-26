namespace Infrastructure.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, OneOf<TResponse, ErrorType>> where TRequest : notnull
{
    private readonly IValidator<TRequest> validator;

    public ValidationBehavior(IValidator<TRequest> validators)
            => this.validator = validators;

    public async Task<OneOf<TResponse, ErrorType>> Handle(TRequest request, RequestHandlerDelegate<OneOf<TResponse, ErrorType>> next, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid is false)
        {
            return new ErrorType(ResponseStatus.BadRequest, validationResult.Errors.First().ErrorMessage);
        }
        return await next();
    }

}
