namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class CompanyQuery
{
    [GraphQLName("companies")]
    public async Task<PaginatedList<CompanyResponse>?> GetCompanyAsync(
        Guid studentId,
        [Service] IStudentDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new GetCompanyHandler(context).Execute(
            new GetCompany { StudentId = studentId }, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
