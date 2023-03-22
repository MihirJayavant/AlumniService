namespace Infrastructure.Commands;

public class AddExamCommand : IRequest<Response<ExamResponse>>
{
    public string ExamName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Year { get; set; }
    public int StudentId { get; set; }
}
