namespace Alumni.Api.GraphQL;

public static class GraphQLResult
{
    public static T Unwrap<T>(OneOf<T, ErrorType> result)
        => result.Match(value => value, error => throw ToException(error));

    private static GraphQLException ToException(ErrorType error)
    {
        var code = error.Status switch
        {
            ResponseStatus.BadRequest => "BAD_REQUEST",
            ResponseStatus.NotFound => "NOT_FOUND",
            ResponseStatus.Conflict => "CONFLICT",
            ResponseStatus.Unauthorized => "UNAUTHORIZED",
            ResponseStatus.Forbidden => "FORBIDDEN",
            _ => "INTERNAL_ERROR"
        };
        return new GraphQLException(ErrorBuilder.New()
            .SetCode(code)
            .SetMessage(code == "INTERNAL_ERROR" ? "An internal error occurred." : error.Message)
            .Build());
    }
}
