namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class StudentQuery
{
    [GraphQLName("students")]
    public async Task<PaginatedList<StudentResponse>?> GetStudentsAsync(
        int pageNumber,
        int pageSize,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var query = new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize };
        var result = await new GetAllStudentHandler(context).Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("student")]
    public async Task<StudentResponse?> GetStudentAsync(
        Guid id,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new GetStudentHandler(context).Execute(new GetStudent { Id = id }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
