using Application.Students;

namespace AlumniBackendServices.Controllers;

public static class StudentController
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/student");

        api.MapGet("/", GetAllAsync).Produces<AllStudentResponse>();
        api.MapGet("/{email}", GetByEmail).Produces<StudentResponse>();
        api.MapPost("/", PostAsync).Produces<StudentResponse>();
    }

    private static async Task<IResult> GetAllAsync(int pageNumber, int pageSize, IMediator mediator)
    {
        var query = new GetAllStudentQuery(pageNumber, pageSize);
        var response = await mediator.Send(query);
        return response.ToServerResult();
    }

    private static async Task<IResult> GetByEmail(string email, IMediator mediator)
    {
        var query = new GetStudentQuery(email);
        var response = await mediator.Send(query);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync([FromBody] AddStudentCommand student, IMediator mediator)
    {
        var response = await mediator.Send(student);
        return response.ToServerResult();
    }

}
