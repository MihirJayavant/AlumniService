namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class FurtherStudyMutation
{
    [GraphQLName("addFurtherStudy")]
    public async Task<FurtherStudyResponse?> AddFurtherStudyAsync(
        AddFurtherStudy input,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new AddFurtherStudyHandler(context).Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
