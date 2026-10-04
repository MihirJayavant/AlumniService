using Alumni.Api.GraphQL;
using Core;
using HotChocolate;
using OneOf;
using Xunit;

namespace Alumni.Api.UnitTests.GraphQL;

public class GraphQLResultTests
{
    [Fact]
    public void Unwrap_WhenSuccessful_ReturnsOriginalPayload()
    {
        var payload = new object();
        OneOf<object, ErrorType> result = payload;

        Assert.Same(payload, GraphQLResult.Unwrap(result));
    }

    [Theory]
    [InlineData(ResponseStatus.BadRequest, "BAD_REQUEST")]
    [InlineData(ResponseStatus.NotFound, "NOT_FOUND")]
    [InlineData(ResponseStatus.Conflict, "CONFLICT")]
    [InlineData(ResponseStatus.Unauthorized, "UNAUTHORIZED")]
    [InlineData(ResponseStatus.Forbidden, "FORBIDDEN")]
    public void Unwrap_WhenExpectedFailure_PreservesMessageAndMapsCode(ResponseStatus status, string code)
    {
        OneOf<string, ErrorType> result = new ErrorType { Status = status, Message = "Specific failure" };

        var exception = Assert.Throws<GraphQLException>(() => GraphQLResult.Unwrap(result));

        var error = Assert.Single(exception.Errors);
        Assert.Equal(code, error.Code);
        Assert.Equal("Specific failure", error.Message);
    }

    [Theory]
    [InlineData(ResponseStatus.InternalError)]
    [InlineData(ResponseStatus.ServiceUnavailable)]
    [InlineData((ResponseStatus)0)]
    [InlineData((ResponseStatus)599)]
    public void Unwrap_WhenUnexpectedFailure_HidesInternalDetails(ResponseStatus status)
    {
        OneOf<string, ErrorType> result = new ErrorType
        {
            Status = status,
            Message = "Private connection string and provider details"
        };

        var exception = Assert.Throws<GraphQLException>(() => GraphQLResult.Unwrap(result));

        var error = Assert.Single(exception.Errors);
        Assert.Equal("INTERNAL_ERROR", error.Code);
        Assert.DoesNotContain("Private", error.Message);
        Assert.False(string.IsNullOrWhiteSpace(error.Message));
    }
}
