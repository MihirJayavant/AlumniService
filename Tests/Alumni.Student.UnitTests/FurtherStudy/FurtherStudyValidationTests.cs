using Alumni.Student.FurtherStudy;
using Core;
using Xunit;

namespace Alumni.Student.UnitTests.FurtherStudy;

public sealed class FurtherStudyValidationTests
{
    [Theory]
    [InlineData("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1")]
    [InlineData("01924023-91e0-7000-8000-000000000001")]
    public void Validate_WhenStudentIdIsNonEmpty_AcceptsAddAndGet(string identifier)
    {
        var studentId = Guid.Parse(identifier);
        var context = new RejectingStudentDbContext();

        var addResult = new AddFurtherStudyHandler(context).Validator.Validate(ValidAddFurtherStudy() with { StudentId = studentId });
        var getResult = new GetFurtherStudyHandler(context).Validator.Validate(new GetFurtherStudy { StudentId = studentId });

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
        var handler = new AddFurtherStudyHandler(context);
        var request = ValidAddFurtherStudy() with { StudentId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(AddFurtherStudy.StudentId), Assert.Single(validation.Errors).PropertyName);
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
        var handler = new GetFurtherStudyHandler(context);
        var request = new GetFurtherStudy { StudentId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(GetFurtherStudy.StudentId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(nameof(AddFurtherStudy.InstituteName), null)]
    [InlineData(nameof(AddFurtherStudy.InstituteName), "")]
    [InlineData(nameof(AddFurtherStudy.InstituteName), " \t\r\n ")]
    [InlineData(nameof(AddFurtherStudy.Degree), null)]
    [InlineData(nameof(AddFurtherStudy.Degree), "")]
    [InlineData(nameof(AddFurtherStudy.Degree), " \t\r\n ")]
    [InlineData(nameof(AddFurtherStudy.Country), null)]
    [InlineData(nameof(AddFurtherStudy.Country), "")]
    [InlineData(nameof(AddFurtherStudy.Country), " \t\r\n ")]
    [InlineData(nameof(AddFurtherStudy.City), null)]
    [InlineData(nameof(AddFurtherStudy.City), "")]
    [InlineData(nameof(AddFurtherStudy.City), " \t\r\n ")]
    public async Task Execute_WhenTextIsMissing_ReturnsBadRequestWithoutDatabaseAccess(string property, string? value)
        => await AssertRejected(SetText(property, value), property);

    [Theory]
    [InlineData(nameof(AddFurtherStudy.InstituteName), 50)]
    [InlineData(nameof(AddFurtherStudy.Degree), 50)]
    [InlineData(nameof(AddFurtherStudy.Country), 30)]
    [InlineData(nameof(AddFurtherStudy.City), 30)]
    public async Task Execute_WhenTrimmedTextExceedsLimit_ReturnsBadRequestWithoutDatabaseAccess(string property, int maximumLength)
        => await AssertRejected(SetText(property, "  " + new string('a', maximumLength + 1) + "  "), property);

    [Theory]
    [InlineData(nameof(AddFurtherStudy.InstituteName), 50)]
    [InlineData(nameof(AddFurtherStudy.Degree), 50)]
    [InlineData(nameof(AddFurtherStudy.Country), 30)]
    [InlineData(nameof(AddFurtherStudy.City), 30)]
    public void Validate_WhenTrimmedTextIsAtLimit_AcceptsWithoutMutatingRequest(string property, int maximumLength)
    {
        var value = "  " + new string('a', maximumLength) + "  ";
        var request = SetText(property, value);

        var result = new AddFurtherStudyHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        var actual = property switch
        {
            nameof(AddFurtherStudy.InstituteName) => request.InstituteName,
            nameof(AddFurtherStudy.Degree) => request.Degree,
            nameof(AddFurtherStudy.Country) => request.Country,
            nameof(AddFurtherStudy.City) => request.City,
            _ => throw new ArgumentOutOfRangeException(nameof(property))
        };
        Assert.Equal(value, actual);
    }

    private static AddFurtherStudy SetText(string property, string? value) => property switch
    {
        nameof(AddFurtherStudy.InstituteName) => ValidAddFurtherStudy() with { InstituteName = value! },
        nameof(AddFurtherStudy.Degree) => ValidAddFurtherStudy() with { Degree = value! },
        nameof(AddFurtherStudy.Country) => ValidAddFurtherStudy() with { Country = value! },
        nameof(AddFurtherStudy.City) => ValidAddFurtherStudy() with { City = value! },
        _ => throw new ArgumentOutOfRangeException(nameof(property))
    };

    private static async Task AssertRejected(AddFurtherStudy request, string property)
    {
        var context = new RejectingStudentDbContext();
        var handler = new AddFurtherStudyHandler(context);

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
    [InlineData(0)]
    [InlineData(10000)]
    public async Task Execute_WhenAdmissionYearIsOutOfRange_ReturnsBadRequestWithoutDatabaseAccess(int value)
        => await AssertRejected(ValidAddFurtherStudy() with { AdmissionYear = value }, nameof(AddFurtherStudy.AdmissionYear));

    [Theory]
    [InlineData(1)]
    [InlineData(9999)]
    public void Validate_WhenAdmissionYearIsAtBoundary_Accepts(int value)
    {
        var request = ValidAddFurtherStudy() with { AdmissionYear = value, PassingYear = 9999 };

        var result = new AddFurtherStudyHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10000)]
    public async Task Execute_WhenPassingYearIsOutOfRange_ReturnsBadRequestWithoutDatabaseAccess(int value)
        => await AssertRejected(ValidAddFurtherStudy() with { PassingYear = value }, nameof(AddFurtherStudy.PassingYear));

    [Theory]
    [InlineData(1)]
    [InlineData(9999)]
    public void Validate_WhenPassingYearIsAtBoundary_Accepts(int value)
    {
        var request = ValidAddFurtherStudy() with { PassingYear = value, AdmissionYear = 1 };

        var result = new AddFurtherStudyHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Execute_WhenPassingPrecedesAdmission_ReturnsBadRequestWithoutDatabaseAccess()
        => await AssertRejected(ValidAddFurtherStudy() with { AdmissionYear = 2024, PassingYear = 2023 }, nameof(AddFurtherStudy.PassingYear));

    [Fact]
    public void Validate_WhenAdmissionAndPassingYearsMatch_Accepts()
    {
        var request = ValidAddFurtherStudy() with { AdmissionYear = 2024, PassingYear = 2024 };

        var result = new AddFurtherStudyHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    private static AddFurtherStudy ValidAddFurtherStudy() => new()
    {
        StudentId = Guid.Parse("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1"),
        FurtherStudyId = Guid.Parse("d33e6ac0-d8bc-440a-a7c2-5ddbd3e599cf"),
        InstituteName = "Alumni University",
        Degree = "Master of Science",
        AdmissionYear = 2022,
        PassingYear = 2024,
        Country = "India",
        City = "Pune"
    };
}
