
namespace Core.Contracts.Response
{
    public class FurtherStudyResponse
    {
        public int FurtherStudyId { get; set; }
        public string InstituteName { get; set; }
        public string Degree { get; set; }
        public int AdmissionYear { get; set; }
        public int PassingYear { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
