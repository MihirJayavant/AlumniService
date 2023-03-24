namespace Infrastructure.Queries;

public class GetFurtherStudyQuery : IRequest<Response<IEnumerable<FurtherStudyResponse>>>
{
    public int StudentId { get; }

    public GetFurtherStudyQuery(int studentId) => StudentId = studentId;
}
