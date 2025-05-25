namespace AlumniBackendServices.Controllers;

public static class FacultyController
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/faculty").WithTags(["Faculty"]).WithOpenApi();

        api.MapGet("/", GetAsync).Produces<PaginatedList<FacultyResponse>>();
        api.MapGet("/{facultyId:guid}", GetByEmailAsync).Produces<FacultyResponse>();
        api.MapPost("/", PostAsync);
        api.MapDelete("/{facultyId:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetAsync(GetAllFaculties query, IFacultyDbContext context, CancellationToken cancellationToken)
    {
        var result = await new GetAllFacultiesHandler(context).Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> GetByEmailAsync(Guid facultyId, IFacultyDbContext context, CancellationToken cancellationToken)
    {
        var query = new GetFaculty { FacultyId = facultyId };
        var result = await new GetFacultyHandler(context).Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddFaculty faculty, IFacultyDbContext context, CancellationToken cancellationToken)
    {
        var result = await new AddFacultyHandler(context).Execute(faculty, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> DeleteAsync(Guid facultyId, IFacultyDbContext context, CancellationToken cancellationToken)
    {
        var query = new DeleteFaculty { FacultyId = facultyId };
        var result = await new DeleteFacultyHandler(context).Execute(query, cancellationToken);
        return result.ToServerResult();
    }
}
