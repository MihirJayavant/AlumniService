namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class ExamMutation
{
    [GraphQLName("addExam")]
    public async Task<ExamResponse?> AddExamAsync(
        AddExam input,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new AddExamHandler(context).Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
