namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class FurtherStudyQuery
{
    [GraphQLName("furtherStudies")]
    public async Task<PaginatedList<FurtherStudyResponse>?> GetFurtherStudyAsync(
        Guid studentId,
        [Service] GetFurtherStudyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(
            new GetFurtherStudy { StudentId = studentId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
