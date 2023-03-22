namespace Core.Contracts.Response;

public class ExamResponse
{
    public string ExamName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Year { get; set; }
    public int StudentId { get; set; }
}
