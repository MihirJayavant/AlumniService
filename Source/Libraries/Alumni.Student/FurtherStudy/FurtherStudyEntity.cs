namespace Alumni.Student.FurtherStudy;

public record FurtherStudyEntity : FurtherStudy
{
    public int StudentId { get; set; }
    public StudentEntity Student { get; set; } = default!;
}
