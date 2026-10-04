namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class FurtherStudyQuery
{
    [GraphQLName("furtherStudies")]
    public async Task<PaginatedList<FurtherStudyResponse>?> GetFurtherStudyAsync(
        Guid studentId,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new GetFurtherStudyHandler(context).Execute(
            new GetFurtherStudy { StudentId = studentId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
