namespace Core.Entities;

public class Exam
{
    public int ExamId { get; set; }
    public string ExamName { get; set; }
    public int Score { get; set; }
    public int Year { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }

}
