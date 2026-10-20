namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class FurtherStudyQuery
{
    [GraphQLName("furtherStudies")]
    public async Task<PaginatedList<FurtherStudyResponse>?> GetFurtherStudyAsync(
        Guid memberId,
        [Service] GetFurtherStudyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(
            new GetFurtherStudy { MemberId = memberId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
