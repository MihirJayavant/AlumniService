using Application.Common.Models;

namespace AlumniBackendServices.Controllers;

public interface IEndpoint
{
    static abstract void Add(WebApplication app);
}

internal record ErrorResponse(string Error);

public class EndpointHelper
{
    public static IResult GetResult<T>(OneOf<T, ErrorType> response)
        => response.Match(
                (p) => Results.Ok(p),
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
