namespace Application.FurtherStudies;

public sealed record FurtherStudyResponse
{
    public required int FurtherStudyId { get; init; }
    public required string InstituteName { get; init; }
    public required string Degree { get; init; }
    public required int AdmissionYear { get; init; }
    public required int PassingYear { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
}
