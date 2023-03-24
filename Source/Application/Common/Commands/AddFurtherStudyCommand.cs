namespace Application.Common.Commands;

public class AddFurtherStudyCommand : IRequest<Response<FurtherStudyResponse>>
{
    public string InstituteName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public int AdmissionYear { get; set; }
    public int PassingYear { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int StudentId { get; set; }
}
