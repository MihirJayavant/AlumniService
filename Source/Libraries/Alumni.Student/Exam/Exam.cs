namespace Alumni.Student.Exam;

public record Exam : IEntity
{
    public required int Id { get; init; }
    public required Guid ExamId { get; init; }
    public required string ExamName { get; init; }
    public required int Score { get; init; }
    public required int Year { get; init; }

}
