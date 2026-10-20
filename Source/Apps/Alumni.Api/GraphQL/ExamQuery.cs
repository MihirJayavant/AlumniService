namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class ExamQuery
{
    [GraphQLName("exams")]
    public async Task<PaginatedList<ExamResponse>?> GetExamAsync(
        Guid memberId,
        [Service] GetExamHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(
            new GetExam { MemberId = memberId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
