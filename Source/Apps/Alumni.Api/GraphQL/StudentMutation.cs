namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class StudentMutation
{
    [GraphQLName("addStudent")]
    public async Task<StudentResponse?> AddStudentAsync(
        AddStudent input,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new AddStudentHandler(context).Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
