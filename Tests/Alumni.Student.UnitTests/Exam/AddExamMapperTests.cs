using Alumni.Student.Exam;
using Xunit;

namespace Alumni.Student.UnitTests.Exam;

public sealed class AddExamMapperTests
{
    [Fact]
    public void ToExam_WhenRequestHasPaddedText_TrimsAndPreservesContentAndInitializesIdentity()
    {
        var request = new AddExam
        {
            StudentId = Guid.NewGuid(),
            ExamId = Guid.NewGuid(),
            ExamName = "  Graduate  Aptitude Test \t",
            Score = 875,
            Year = 2024,
        };

        var entity = request.ToExam();

        Assert.Equal("Graduate  Aptitude Test", entity.ExamName);
        Assert.Equal("  Graduate  Aptitude Test \t", request.ExamName);
        Assert.Equal(request.Score, entity.Score);
        Assert.Equal(request.Year, entity.Year);
        Assert.Equal(0, entity.Id);
        Assert.NotEqual(Guid.Empty, entity.ExamId);
        Assert.NotEqual(request.ExamId, entity.ExamId);
        Assert.Equal(7, entity.ExamId.Version);
    }
}
