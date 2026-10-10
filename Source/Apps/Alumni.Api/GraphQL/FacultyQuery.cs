namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class FacultyQuery
{
    [GraphQLName("faculties")]
    public async Task<PaginatedList<FacultyResponse>?> GetFacultiesAsync(
        int pageNumber,
        int pageSize,
        [Service] GetAllFacultiesHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllFaculties { PageNumber = pageNumber, PageSize = pageSize };
        var result = await handler.Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("faculty")]
    public async Task<FacultyResponse?> GetFacultyAsync(
        Guid facultyId,
        [Service] GetFacultyHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetFaculty { FacultyId = facultyId };
        var result = await handler.Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
