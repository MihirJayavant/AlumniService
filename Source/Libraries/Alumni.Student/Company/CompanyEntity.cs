namespace Alumni.Student.Company;

public record CompanyEntity : Company
{
    public required int StudentId { get; init; }
    public required StudentEntity Student { get; init; }
}
