using Application.Common.Models;
using OneOf;

namespace AlumniBackendServices.Controllers;

public interface IEndpoint
{
    static abstract void Add(WebApplication app);
}

public class EndpointHelper
{
    public static IResult GetResult<T>(OneOf<T, ErrorType> response)
        => response.Match(
                (p) => Results.Ok(p),
                (e) => e.Status switch
                {
                    ResponseStatus.BadRequest => Results.BadRequest(e.Message),
                    ResponseStatus.NotFound => Results.NotFound(e.Message),
                    ResponseStatus.Conflict => Results.Conflict(e.Message),
                    ResponseStatus.InternalError => Results.Problem(detail: e.Message, statusCode: 500),
                    _ => Results.BadRequest("Bad request")
                }
            );
}
