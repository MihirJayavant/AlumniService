namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class FacultyQuery
{
    [GraphQLName("faculties")]
    public async Task<PaginatedList<FacultyResponse>?> GetFacultiesAsync(
        int pageNumber,
        int pageSize,
        [Service] IFacultyDbContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetAllFaculties { PageNumber = pageNumber, PageSize = pageSize };
        var result = await new GetAllFacultiesHandler(context).Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("faculty")]
    public async Task<FacultyResponse?> GetFacultyAsync(
        Guid facultyId,
        [Service] IFacultyDbContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetFaculty { FacultyId = facultyId };
        var result = await new GetFacultyHandler(context).Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
