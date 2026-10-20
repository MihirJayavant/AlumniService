namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class MemberMutation
{
    [GraphQLName("addMember")]
    public async Task<MemberResponse?> AddMemberAsync(
        AddMember input,
        [Service] AddMemberHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
