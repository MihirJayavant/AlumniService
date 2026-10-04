namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class CompanyMutation
{
    [GraphQLName("addCompany")]
    public async Task<CompanyResponse?> AddCompanyAsync(
        AddCompany input,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new AddCompanyHandler(context).Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
