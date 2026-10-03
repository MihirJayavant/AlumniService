namespace Alumni.Api.Controllers;

internal sealed record ErrorResponse(string Error);

public static class ResultHelper
{
    extension<T>(OneOf<T, ErrorType> response)
    {
        public IResult ToServerResult()
        => response.Match(
                Results.Ok,
                (e) => e.Status switch
                {
                    ResponseStatus.BadRequest => Results.BadRequest(new ErrorResponse(e.Message)),
                    ResponseStatus.NotFound => Results.NotFound(new ErrorResponse(e.Message)),
                    ResponseStatus.Conflict => Results.Conflict(new ErrorResponse(e.Message)),
                    ResponseStatus.Unauthorized => Results.Unauthorized(),
                    ResponseStatus.InternalError => Results.Problem(detail: e.Message, statusCode: 500),
                    _ when (int)e.Status is >= 400 and <= 599 =>
                        Results.Json(new ErrorResponse(e.Message), statusCode: (int)e.Status),
                    _ => Results.Problem(detail: e.Message, statusCode: 500)
                }
            );
    }
}
