namespace Infrastructure.Queries;

public class GetCompanyQuery : IRequest<Response<IEnumerable<CompanyResponse>>>
{
    public int StudentId { get; }

    public GetCompanyQuery(int studentId) => StudentId = studentId;
}
