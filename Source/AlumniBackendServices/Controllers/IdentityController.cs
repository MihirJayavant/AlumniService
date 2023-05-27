using Application.Common.Interfaces;
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

    public static async Task<IResult> StudentRegistration([FromBody] IdentityRequest body, IIdentityService identity)
    {
        var result = await identity.RegisterStudent(body.Email, body.Password);
        return EndpointHelper.GetResult(result);
    }

    public static async Task<IResult> StudentLogin([FromBody] IdentityRequest body, IIdentityService identity)
    {
        var result = await identity.StudentLogin(body.Email, body.Password);
        return EndpointHelper.GetResult(result);
    }

}
