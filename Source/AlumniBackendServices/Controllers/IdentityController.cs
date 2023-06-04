using Application.Identity;

namespace AlumniBackendServices.Controllers;

public class IdentityController : IEndpoint
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/auth");

        api.MapPost("/registration", StudentRegistration);
        api.MapPost("/login", StudentLogin);
    }

    public static async Task<IResult> StudentRegistration([FromBody] AddStudentIdentityCommand body, IMediator mediator)
    {
        var result = await mediator.Send(body);
        return EndpointHelper.GetResult(result);
    }

    public static async Task<IResult> StudentLogin([FromBody] StudentLoginQuery body, IMediator mediator)
    {
        var result = await mediator.Send(body);
        return EndpointHelper.GetResult(result);
    }

}
