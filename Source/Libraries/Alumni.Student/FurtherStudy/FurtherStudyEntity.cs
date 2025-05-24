namespace Alumni.Student.FurtherStudy;

public record FurtherStudyEntity : FurtherStudy
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;
}
