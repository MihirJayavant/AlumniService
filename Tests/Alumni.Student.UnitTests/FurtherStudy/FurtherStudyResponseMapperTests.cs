using Alumni.Student.FurtherStudy;
using Xunit;
using FurtherStudyModel = Alumni.Student.FurtherStudy.FurtherStudy;

namespace Alumni.Student.UnitTests.FurtherStudy;

public sealed class FurtherStudyResponseMapperTests
{
    [Fact]
    public void ToFurtherStudyResponse_WhenFurtherStudyHasValues_PreservesAllExposedFields()
    {
        var furtherStudy = new FurtherStudyModel
        {
            Id = 44,
            FurtherStudyId = Guid.Parse("d33e6ac0-d8bc-440a-a7c2-5ddbd3e599cf"),
            InstituteName = "Alumni University",
            Degree = "Master of Science",
            AdmissionYear = 2022,
            PassingYear = 2024,
            Country = "India",
            City = "Pune"
        };

        var response = furtherStudy.ToFurtherStudyResponse();

        Assert.Equal(furtherStudy.FurtherStudyId, response.FurtherStudyId);
        Assert.Equal(furtherStudy.InstituteName, response.InstituteName);
        Assert.Equal(furtherStudy.Degree, response.Degree);
        Assert.Equal(furtherStudy.AdmissionYear, response.AdmissionYear);
        Assert.Equal(furtherStudy.PassingYear, response.PassingYear);
        Assert.Equal(furtherStudy.Country, response.Country);
        Assert.Equal(furtherStudy.City, response.City);
    }
}
