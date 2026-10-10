namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class StudentMutation
{
    [GraphQLName("addStudent")]
    public async Task<StudentResponse?> AddStudentAsync(
        AddStudent input,
        [Service] AddStudentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
