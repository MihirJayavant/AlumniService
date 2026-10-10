namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class FurtherStudyMutation
{
    [GraphQLName("addFurtherStudy")]
    public async Task<FurtherStudyResponse?> AddFurtherStudyAsync(
        AddFurtherStudy input,
        [Service] AddFurtherStudyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
