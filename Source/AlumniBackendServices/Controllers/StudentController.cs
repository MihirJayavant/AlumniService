using AlumniBackendServices.Models;
using Application.Students;

namespace AlumniBackendServices.Controllers;

public class StudentController : IEndpoint
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/student");

        api.MapGet("/", GetAllAsync).Produces<AllStudentResponse>();
        api.MapGet("/{email}", GetByEmail).Produces<StudentResponse>();
        api.MapPost("/", PostAsync).Produces<StudentResponse>();
    }

    public static async Task<IResult> GetAllAsync(PageQuery request, IMediator mediator)
    {
        var query = new GetAllStudentQuery(request.PageNumber, request.PageSize);
        var response = await mediator.Send(query);
        return EndpointHelper.GetResult(response);
    }

    public static async Task<IResult> GetByEmail(string email, IMediator mediator)
    {
        var query = new GetStudentQuery(email);
        var response = await mediator.Send(query);
        return EndpointHelper.GetResult(response);
    }

    public static async Task<IResult> PostAsync([FromBody] AddStudentCommand student, IMediator mediator)
    {
        var response = await mediator.Send(student);
        return EndpointHelper.GetResult(response);
    }

}
