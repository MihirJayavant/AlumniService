namespace Core.Entities;

public class FurtherStudy
{
    public int FurtherStudyId { get; set; }
    public string InstituteName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public int AdmissionYear { get; set; }
    public int PassingYear { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;
}
