using Alumni.Student.FurtherStudy;
using Xunit;

namespace Alumni.Student.UnitTests.FurtherStudy;

public sealed class AddFurtherStudyMapperTests
{
    [Fact]
    public void ToFurtherStudy_WhenRequestHasPaddedText_TrimsAndPreservesContentAndInitializesIdentity()
    {
        var request = new AddFurtherStudy
        {
            StudentId = Guid.NewGuid(),
            FurtherStudyId = Guid.NewGuid(),
            InstituteName = "  École  University \t",
            Degree = "  Master  of Science \t",
            Country = "  New  Zealand \t",
            City = "  São  Paulo \t",
            AdmissionYear = 2022,
            PassingYear = 2024,
        };

        var entity = request.ToFurtherStudy();

        Assert.Equal("École  University", entity.InstituteName);
        Assert.Equal("  École  University \t", request.InstituteName);
        Assert.Equal("Master  of Science", entity.Degree);
        Assert.Equal("  Master  of Science \t", request.Degree);
        Assert.Equal("New  Zealand", entity.Country);
        Assert.Equal("  New  Zealand \t", request.Country);
        Assert.Equal("São  Paulo", entity.City);
        Assert.Equal("  São  Paulo \t", request.City);
        Assert.Equal(request.AdmissionYear, entity.AdmissionYear);
        Assert.Equal(request.PassingYear, entity.PassingYear);
        Assert.Equal(0, entity.Id);
        Assert.NotEqual(Guid.Empty, entity.FurtherStudyId);
        Assert.NotEqual(request.FurtherStudyId, entity.FurtherStudyId);
        Assert.Equal(7, entity.FurtherStudyId.Version);
    }
}
