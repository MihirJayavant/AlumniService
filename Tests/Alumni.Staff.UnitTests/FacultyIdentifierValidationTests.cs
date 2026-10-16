using Core;
using Xunit;

namespace Alumni.Staff.UnitTests;

public sealed class FacultyIdentifierValidationTests
{
    [Theory]
    [InlineData("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1")]
    [InlineData("01924023-91e0-7000-8000-000000000001")]
    public void Validate_WhenFacultyIdIsNonEmpty_AcceptsGetAndDelete(string identifier)
    {
        var facultyId = Guid.Parse(identifier);
        var context = new RejectingFacultyDbContext();

        var getResult = new GetStaffMemberHandler(context).Validator.Validate(new GetStaffMember { FacultyId = facultyId });
        var deleteResult = new DeleteStaffMemberHandler(context).Validator.Validate(new DeleteStaffMember { FacultyId = facultyId });

        Assert.True(getResult.IsValid);
        Assert.True(deleteResult.IsValid);
        Assert.Equal(0, context.QueryAccessCount);
    }

    [Fact]
    public async Task Execute_WhenGetFacultyIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingFacultyDbContext();
        var handler = new GetStaffMemberHandler(context);
        var request = new GetStaffMember { FacultyId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(GetStaffMember.FacultyId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenDeleteFacultyIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingFacultyDbContext();
        var handler = new DeleteStaffMemberHandler(context);
        var request = new DeleteStaffMember { FacultyId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(DeleteStaffMember.FacultyId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }
}
