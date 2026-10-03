using Core;
using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class StudentPaginationValidationTests
{
    [Fact]
    public void Validate_WhenPaginationUsesDefaults_AcceptsFirstPageOfTen()
    {
        var request = new GetAllStudent();

        var result = new GetAllStudentValidator().Validate(request);

        Assert.Equal(1, request.PageNumber);
        Assert.Equal(10, request.PageSize);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 100)]
    [InlineData(2, 10)]
    [InlineData(int.MaxValue, 1)]
    [InlineData(21474837, 100)]
    public void Validate_WhenPaginationIsWithinLimits_AcceptsRequest(int pageNumber, int pageSize)
    {
        var result = new GetAllStudentValidator().Validate(
            new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize });

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0, 10, "PageNumber")]
    [InlineData(-1, 10, "PageNumber")]
    [InlineData(int.MinValue, 10, "PageNumber")]
    [InlineData(1, 0, "PageSize")]
    [InlineData(1, -1, "PageSize")]
    [InlineData(1, 101, "PageSize")]
    [InlineData(1, int.MaxValue, "PageSize")]
    public void Validate_WhenPaginationIsOutsideLimits_RejectsRelevantField(
        int pageNumber, int pageSize, string propertyName)
    {
        var result = new GetAllStudentValidator().Validate(
            new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize });

        Assert.False(result.IsValid);
        Assert.Equal(propertyName, Assert.Single(result.Errors).PropertyName);
    }

    [Theory]
    [InlineData(21474838, 100)]
    [InlineData(int.MaxValue, 2)]
    public void Validate_WhenPageOffsetWouldOverflow_RejectsRequest(int pageNumber, int pageSize)
    {
        var result = new GetAllStudentValidator().Validate(
            new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize });

        var error = Assert.Single(result.Errors);
        Assert.Equal(nameof(GetAllStudent.PageNumber), error.PropertyName);
        Assert.Equal("The requested page offset is too large.", error.ErrorMessage);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 101)]
    [InlineData(21474838, 100)]
    public async Task Execute_WhenPaginationIsInvalid_ReturnsBadRequestWithoutDatabaseAccess(
        int pageNumber, int pageSize)
    {
        var context = new RejectingStudentDbContext();
        var handler = new GetAllStudentHandler(context);
        var request = new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize };

        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(handler.Validator.Validate(request).Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }
}
