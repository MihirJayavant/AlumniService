using AlumniBackendServices.Services;
using Application.Common.Models;
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

    public static async Task<IResult> StudentRegistration([FromBody] StudentRegistration body, IIdentityService identity)
    {
        var result = await identity.RegisterStudent(body.Email, body.Password);
        return EndpointHelper.GetResult(result);
    }

    public static async Task<IResult> StudentLogin([FromBody] StudentRegistration body, IIdentityService identity)
    {
        var result = await identity.StudentLogin(body.Email, body.Password);
        return EndpointHelper.GetResult(result);
    }

}
