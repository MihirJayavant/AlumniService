namespace Infrastructure.Queries;

public class GetCompanyGraphQL : IRequest<IQueryable<Company>>
{
    public int StudentId { get; }

    public GetCompanyGraphQL(int studentId) => StudentId = studentId;
}
