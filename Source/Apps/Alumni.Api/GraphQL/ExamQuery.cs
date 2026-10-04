namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class ExamQuery
{
    [GraphQLName("exams")]
    public async Task<PaginatedList<ExamResponse>?> GetExamAsync(
        Guid studentId,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new GetExamHandler(context).Execute(
            new GetExam { StudentId = studentId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
