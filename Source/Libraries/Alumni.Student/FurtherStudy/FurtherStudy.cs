namespace Alumni.Student.FurtherStudy;

public record FurtherStudy : IEntity
{
    public required int Id { get; init; }
    public required Guid FurtherStudyId { get; init; }
    public required string InstituteName { get; init; }
    public required string Degree { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
}
