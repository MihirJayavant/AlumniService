namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class CompanyQuery
{
    [GraphQLName("companies")]
    public async Task<PaginatedList<CompanyResponse>?> GetCompanyAsync(
        Guid studentId,
        [Service] GetCompanyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(
            new GetCompany { StudentId = studentId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
