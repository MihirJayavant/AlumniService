namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class StudentQuery
{
    [GraphQLName("students")]
    public async Task<PaginatedList<StudentResponse>?> GetStudentsAsync(
        int pageNumber,
        int pageSize,
        [Service] GetAllStudentHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize };
        var result = await handler.Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("student")]
    public async Task<StudentResponse?> GetStudentAsync(
        Guid id,
        [Service] GetStudentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(new GetStudent { Id = id }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
