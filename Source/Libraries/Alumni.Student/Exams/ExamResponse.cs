namespace Application.Exams;

public sealed record ExamResponse
{
    public required string ExamName { get; init; }
    public required int Score { get; init; }
    public required int Year { get; init; }
    public required int StudentId { get; init; }
}
