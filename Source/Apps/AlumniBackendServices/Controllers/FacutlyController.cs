
using Application.Faculties;
namespace AlumniBackendServices.Controllers;

public static class FacultyController
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/faculty");

        api.MapGet("/", GetAsync).Produces<IEnumerable<FacultyResponse>>()
                .WithOpenApi();
        api.MapGet("/{email}", GetByEmailAsync).Produces<FacultyResponse>();
        api.MapPost("/", PostAsync);
        api.MapDelete("/{facultyId}", DeleteAsync);
    }

    private static async Task<IResult> GetAsync(int pageNumber, int pageSize, IMediator mediator)
    {
        var request = new GetAllFacultiesQuery(pageNumber, pageSize);
        var result = await mediator.Send(request);
        return result.ToServerResult();
    }

    private static async Task<IResult> GetByEmailAsync(string email, IMediator mediator)
    {
        var query = new GetFacultyQuery(email);
        var result = await mediator.Send(query);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync([FromBody] AddFacultyCommand faculty, IMediator mediator)
    {
        var result = await mediator.Send(faculty);
        return result.ToServerResult();
    }

    private static async Task<IResult> DeleteAsync(int facultyId, IMediator mediator)
    {
        var query = new DeleteFacultyCommand(facultyId);
        var result = await mediator.Send(query);
        return result.ToServerResult();
    }
}
