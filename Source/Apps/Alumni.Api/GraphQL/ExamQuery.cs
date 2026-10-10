namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class ExamQuery
{
    [GraphQLName("exams")]
    public async Task<PaginatedList<ExamResponse>?> GetExamAsync(
        Guid studentId,
        [Service] GetExamHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(
            new GetExam { StudentId = studentId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
