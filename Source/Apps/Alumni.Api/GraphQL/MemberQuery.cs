namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class MemberQuery
{
    [GraphQLName("members")]
    public async Task<PaginatedList<MemberResponse>?> GetMembersAsync(
        int pageNumber,
        int pageSize,
        [Service] GetMembersHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetMembers { PageNumber = pageNumber, PageSize = pageSize };
        var result = await handler.Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("member")]
    public async Task<MemberResponse?> GetMemberAsync(
        Guid id,
        [Service] GetMemberHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(new GetMember { Id = id }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
