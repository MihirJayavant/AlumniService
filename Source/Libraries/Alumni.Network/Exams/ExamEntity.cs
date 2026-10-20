namespace Alumni.Network.Exams;

public record ExamEntity : Exam
{
    public int MemberId { get; set; }
    public MemberEntity Member { get; set; } = default!;
}
