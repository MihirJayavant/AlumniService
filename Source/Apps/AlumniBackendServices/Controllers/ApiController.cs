namespace AlumniBackendServices.Controllers;

internal record ErrorResponse(string Error);

public static class EndpointHelper
{
    public static IResult ToServerResult<T>(this OneOf<T, ErrorType> response)
        => response.Match(
                Results.Ok,
                (e) => e.Status switch
                {
                    ResponseStatus.BadRequest => Results.BadRequest(new ErrorResponse(e.Message)),
                    ResponseStatus.NotFound => Results.NotFound(new ErrorResponse(e.Message)),
                    ResponseStatus.Conflict => Results.Conflict(new ErrorResponse(e.Message)),
                    ResponseStatus.InternalError => Results.Problem(detail: e.Message, statusCode: 500),
                    _ => Results.BadRequest("Bad request")
                }
            );
}
