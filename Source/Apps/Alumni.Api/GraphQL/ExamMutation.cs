namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class ExamMutation
{
    [GraphQLName("addExam")]
    public async Task<ExamResponse?> AddExamAsync(
        AddExam input,
        [Service] AddExamHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
