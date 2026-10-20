using Core;
using Xunit;

namespace Alumni.Network.UnitTests;

public sealed class MemberIdentifierValidationTests
{
    [Theory]
    [InlineData("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1")]
    [InlineData("01924023-91e0-7000-8000-000000000001")]
    public void Validate_WhenIdIsNonEmpty_AcceptsRequest(string identifier)
    {
        var context = new RejectingMemberDbContext();

        var result = new GetMemberHandler(context).Validator.Validate(new GetMember { Id = Guid.Parse(identifier) });

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingMemberDbContext();
        var handler = new GetMemberHandler(context);
        var request = new GetMember { Id = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        var error = Assert.Single(validation.Errors);
        Assert.Equal(nameof(GetMember.Id), error.PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(error.ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }
}
