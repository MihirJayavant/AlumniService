namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class CompanyQuery
{
    [GraphQLName("companies")]
    public async Task<PaginatedList<CompanyResponse>?> GetCompanyAsync(
        Guid memberId,
        [Service] GetCompanyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(
            new GetCompany { MemberId = memberId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
