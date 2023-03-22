namespace AlumniBackendServices.Controllers;

public class FacultyController : IEndpoint
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/faculty");

        api.MapGet("/", GetAsync).Produces<IEnumerable<FacultyResponse>>();
        api.MapGet("/{email}", GetByEmailAsync).Produces<FacultyResponse>();
        api.MapPost("/", PostAsync);
        api.MapDelete("/{facultyId]}", DeleteAsync);
    }

    public static async Task<IResult> GetAsync(IMediator mediator)
    {
        var query = new GetAllFacultiesQuery();
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> GetByEmailAsync(string email, IMediator mediator)
    {
        var query = new GetFacultyQuery(email);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> PostAsync(AddFacultyCommand faculty, IMediator mediator)
    {
        await mediator.Send(faculty);
        return Results.Ok();
    }

    public static async Task<IResult> DeleteAsync(int facultyId, IMediator mediator)
    {
        var query = new DeleteFacultyCommand(facultyId);
        await mediator.Send(query);
        return Results.Ok();
    }
}
