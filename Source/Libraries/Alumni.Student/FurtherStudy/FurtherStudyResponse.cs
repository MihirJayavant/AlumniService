namespace Alumni.Student.FurtherStudy;

[RecordView(typeof(FurtherStudy), nameof(FurtherStudy.Id))]
public sealed partial record FurtherStudyResponse
{

}

public static class FurtherStudyResponseMapper
{
    public static FurtherStudyResponse ToFurtherStudyResponse(this FurtherStudy furtherStudy) =>
        new()
        {
            FurtherStudyId = furtherStudy.FurtherStudyId,
            InstituteName = furtherStudy.InstituteName,
            Degree = furtherStudy.Degree,
            AdmissionYear = furtherStudy.AdmissionYear,
            PassingYear = furtherStudy.PassingYear,
            Country = furtherStudy.Country,
            City = furtherStudy.City,
        };
}
