using Alumni.Student.Exam;
using Xunit;
using ExamModel = Alumni.Student.Exam.Exam;

namespace Alumni.Student.UnitTests.Exam;

public sealed class ExamResponseMapperTests
{
    [Fact]
    public void ToExamResponse_WhenExamHasValues_PreservesAllExposedFields()
    {
        var exam = new ExamModel
        {
            Id = 43,
            ExamId = Guid.Parse("31211cb0-66a8-4dc7-bb5c-3de18b3e7457"),
            ExamName = "Graduate Aptitude Test",
            Score = 875,
            Year = 2024
        };

        var response = exam.ToExamResponse();

        Assert.Equal(exam.ExamId, response.ExamId);
        Assert.Equal(exam.ExamName, response.ExamName);
        Assert.Equal(exam.Score, response.Score);
        Assert.Equal(exam.Year, response.Year);
    }
}
