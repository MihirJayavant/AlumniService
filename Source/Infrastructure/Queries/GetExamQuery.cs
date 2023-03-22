namespace Infrastructure.Queries;

public class GetExamQuery : IRequest<Response<IEnumerable<ExamResponse>>>
{
    public int StudentId { get; }

    public GetExamQuery(int studentId) => StudentId = studentId;
}
