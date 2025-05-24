namespace Alumni.Student.Exam;

public sealed record Exam : IEntity
{
    public required int Id { get; init; }
    public required Guid ExamId { get; init; }
    public required string ExamName { get; init; }
    public required int Score { get; init; }
    public required int Year { get; init; }
    public required int StudentId { get; init; }
    public required Student Student { get; init; }
}
