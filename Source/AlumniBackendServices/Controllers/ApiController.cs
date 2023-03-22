#pragma warning disable IDE1006

using Microsoft.AspNetCore.Mvc;

namespace AlumniBackendServices.Controllers;


[ApiController]
[ProducesErrorResponseType(typeof(ErrorResponse))]
[Produces("application/json")]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected readonly IMediator mediator;

    protected ApiController(IMediator mediator) => this.mediator = mediator;

    protected IActionResult GetResult<T>(Response<T> response)
            => response.Status switch
            {
                ResponseStatus.Success => Ok(response.Result),
                ResponseStatus.Created => Ok(response.Result),
                ResponseStatus.BadRequest => BadRequest(response.Error),
                ResponseStatus.NotFound => NotFound(response.Error),
                _ => throw new System.NotImplementedException(),
            };

}


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
