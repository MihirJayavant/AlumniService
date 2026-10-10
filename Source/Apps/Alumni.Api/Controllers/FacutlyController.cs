namespace Alumni.Api.Controllers;

public sealed class FacultyController : IEndpoint
{

    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/faculty").WithTags(["Faculty"]);

        api.MapGet("/", GetAsync).Produces<PaginatedList<FacultyResponse>>();
        api.MapGet("/{facultyId:guid}", GetByEmailAsync).Produces<FacultyResponse>();
        api.MapPost("/", PostAsync);
        api.MapDelete("/{facultyId:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetAsync(int pageNumber, int pageSize, GetAllFacultiesHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetAllFaculties { PageNumber = pageNumber, PageSize = pageSize };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> GetByEmailAsync(Guid facultyId, GetFacultyHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetFaculty { FacultyId = facultyId };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddFaculty faculty, AddFacultyHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.Execute(faculty, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> DeleteAsync(Guid facultyId, DeleteFacultyHandler handler, CancellationToken cancellationToken)
    {
        var query = new DeleteFaculty { FacultyId = facultyId };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }
}
