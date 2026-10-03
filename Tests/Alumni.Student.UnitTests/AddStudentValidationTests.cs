using Core;
using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class AddStudentValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n ")]
    [InlineData("student")]
    [InlineData("student@")]
    [InlineData("@example.com")]
    [InlineData("student@example")]
    [InlineData("student..name@example.com")]
    [InlineData("student name@example.com")]
    [InlineData("student@-example.com")]
    public async Task Execute_WhenEmailIsMissingOrMalformed_ReturnsBadRequestWithoutDatabaseAccess(string? email)
    {
        var context = new RejectingStudentDbContext();
        var handler = new AddStudentHandler(context);
        var request = StudentTestData.ValidAddStudent() with { Email = email! };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        var error = Assert.Single(validation.Errors);
        Assert.Equal(nameof(AddStudent.Email), error.PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(error.ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData("Student.Name@Example.COM")]
    [InlineData("  Student.Name@Example.COM \t")]
    [InlineData("student.name+alumni@example.com")]
    [InlineData("student@xn--bcher-kva.example")]
    public void Validate_WhenEmailMeetsPolicy_AcceptsWithoutMutatingInput(string email)
    {
        var context = new RejectingStudentDbContext();
        var request = StudentTestData.ValidAddStudent() with { Email = email };

        var result = new AddStudentHandler(context).Validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal(email, request.Email);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(100, false, true)]
    [InlineData(100, true, true)]
    [InlineData(101, false, false)]
    [InlineData(101, true, false)]
    public async Task Validate_WhenEmailLengthIsAtBoundary_UsesNormalizedLength(int length, bool padded, bool valid)
    {
        var email = new string('a', 64) + "@" + new string('b', length - 69) + ".com";
        _ = new Email(email);
        var request = StudentTestData.ValidAddStudent() with { Email = padded ? $"  {email} \t" : email };
        var context = new RejectingStudentDbContext();
        var handler = new AddStudentHandler(context);

        var result = handler.Validator.Validate(request);

        Assert.Equal(valid, result.IsValid);
        if (!valid)
        {
            var error = Assert.Single(result.Errors);
            Assert.Equal(nameof(AddStudent.Email), error.PropertyName);
            Assert.Equal("Email must be at most 100 characters.", error.ErrorMessage);
            var response = await handler.Execute(request, TestContext.Current.CancellationToken);
            Assert.True(response.IsT1);
            Assert.Equal(ResponseStatus.BadRequest, response.AsT1.Status);
            Assert.Equal(error.ErrorMessage, response.AsT1.Message);
        }

        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }
}
