namespace AlumniBackendServices.Controllers;

public interface IEndpoint
{
    static abstract void Add(WebApplication app);
}

public class EndpointHelper
{
    public static IResult GetResult<T>(Response<T> response)
        => response.Status switch
        {
            ResponseStatus.Success => Results.Ok(response.Result),
            ResponseStatus.Created => Results.Ok(response.Result),
            ResponseStatus.BadRequest => Results.BadRequest(response.Error),
            ResponseStatus.NotFound => Results.NotFound(response.Error),
            _ => throw new NotImplementedException(),
        };
}
