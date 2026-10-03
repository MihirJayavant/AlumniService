using Alumni.Student.Exam;
using Core;
using Xunit;

namespace Alumni.Student.UnitTests.Exam;

public sealed class ExamValidationTests
{
    [Theory]
    [InlineData("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1")]
    [InlineData("01924023-91e0-7000-8000-000000000001")]
    public void Validate_WhenStudentIdIsNonEmpty_AcceptsAddAndGet(string identifier)
    {
        var studentId = Guid.Parse(identifier);
        var context = new RejectingStudentDbContext();

        var addResult = new AddExamHandler(context).Validator.Validate(ValidAddExam() with { StudentId = studentId });
        var getResult = new GetExamHandler(context).Validator.Validate(new GetExam { StudentId = studentId });

        Assert.True(addResult.IsValid);
        Assert.Empty(addResult.Errors);
        Assert.True(getResult.IsValid);
        Assert.Empty(getResult.Errors);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenAddStudentIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingStudentDbContext();
        var handler = new AddExamHandler(context);
        var request = ValidAddExam() with { StudentId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(AddExam.StudentId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenGetStudentIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingStudentDbContext();
        var handler = new GetExamHandler(context);
        var request = new GetExam { StudentId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(GetExam.StudentId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(nameof(AddExam.ExamName), null)]
    [InlineData(nameof(AddExam.ExamName), "")]
    [InlineData(nameof(AddExam.ExamName), " \t\r\n ")]
    public async Task Execute_WhenTextIsMissing_ReturnsBadRequestWithoutDatabaseAccess(string property, string? value)
        => await AssertRejected(SetText(property, value), property);

    [Theory]
    [InlineData(nameof(AddExam.ExamName), 100)]
    public async Task Execute_WhenTrimmedTextExceedsLimit_ReturnsBadRequestWithoutDatabaseAccess(string property, int maximumLength)
        => await AssertRejected(SetText(property, "  " + new string('a', maximumLength + 1) + "  "), property);

    [Theory]
    [InlineData(nameof(AddExam.ExamName), 100)]
    public void Validate_WhenTrimmedTextIsAtLimit_AcceptsWithoutMutatingRequest(string property, int maximumLength)
    {
        var value = "  " + new string('a', maximumLength) + "  ";
        var request = SetText(property, value);

        var result = new AddExamHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        var actual = property switch
        {
            nameof(AddExam.ExamName) => request.ExamName,
            _ => throw new ArgumentOutOfRangeException(nameof(property))
        };
        Assert.Equal(value, actual);
    }

    private static AddExam SetText(string property, string? value) => property switch
    {
        nameof(AddExam.ExamName) => ValidAddExam() with { ExamName = value! },
        _ => throw new ArgumentOutOfRangeException(nameof(property))
    };

    private static async Task AssertRejected(AddExam request, string property)
    {
        var context = new RejectingStudentDbContext();
        var handler = new AddExamHandler(context);

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Contains(validation.Errors, error => error.PropertyName == property);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(32768)]
    public async Task Execute_WhenScoreIsOutOfRange_ReturnsBadRequestWithoutDatabaseAccess(int value)
        => await AssertRejected(ValidAddExam() with { Score = value }, nameof(AddExam.Score));

    [Theory]
    [InlineData(0)]
    [InlineData(32767)]
    public void Validate_WhenScoreIsAtBoundary_Accepts(int value)
    {
        var request = ValidAddExam() with { Score = value };

        var result = new AddExamHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10000)]
    public async Task Execute_WhenYearIsOutOfRange_ReturnsBadRequestWithoutDatabaseAccess(int value)
        => await AssertRejected(ValidAddExam() with { Year = value }, nameof(AddExam.Year));

    [Theory]
    [InlineData(1)]
    [InlineData(9999)]
    public void Validate_WhenYearIsAtBoundary_Accepts(int value)
    {
        var request = ValidAddExam() with { Year = value };

        var result = new AddExamHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    private static AddExam ValidAddExam() => new()
    {
        StudentId = Guid.Parse("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1"),
        ExamId = Guid.Parse("31211cb0-66a8-4dc7-bb5c-3de18b3e7457"),
        ExamName = "Graduate Aptitude Test",
        Score = 875,
        Year = 2024
    };
}
