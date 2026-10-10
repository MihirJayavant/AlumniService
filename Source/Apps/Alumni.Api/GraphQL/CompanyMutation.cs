namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class CompanyMutation
{
    [GraphQLName("addCompany")]
    public async Task<CompanyResponse?> AddCompanyAsync(
        AddCompany input,
        [Service] AddCompanyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
