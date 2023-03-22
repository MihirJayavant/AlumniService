using AlumniBackendServices.Services;
using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AlumniBackendServices.Controllers;

public class IdentityController : IEndpoint
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/student");

        api.MapPost("/registration", StudentRegistration);
        api.MapPost("/login", StudentLogin);
    }

    public static async Task<IResult> StudentRegistration(IIdentityService identity, StudentRegistration body)
    {
        var result = await identity.RegisterStudent(body.Email, body.Password);
        return EndpointHelper.GetResult(result);
    }

    [HttpPost("student/login")]
    public static async Task<IResult> StudentLogin(IIdentityService identity, StudentRegistration body)
    {
        var result = await identity.StudentLogin(body.Email, body.Password);
        return EndpointHelper.GetResult(result);
    }

}
