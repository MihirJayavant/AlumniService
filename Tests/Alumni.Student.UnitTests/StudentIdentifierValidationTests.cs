using Core;
using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class StudentIdentifierValidationTests
{
    [Theory]
    [InlineData("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1")]
    [InlineData("01924023-91e0-7000-8000-000000000001")]
    public void Validate_WhenIdIsNonEmpty_AcceptsRequest(string identifier)
    {
        var context = new RejectingStudentDbContext();

        var result = new GetStudentHandler(context).Validator.Validate(new GetStudent { Id = Guid.Parse(identifier) });

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingStudentDbContext();
        var handler = new GetStudentHandler(context);
        var request = new GetStudent { Id = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        var error = Assert.Single(validation.Errors);
        Assert.Equal(nameof(GetStudent.Id), error.PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(error.ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }
}
