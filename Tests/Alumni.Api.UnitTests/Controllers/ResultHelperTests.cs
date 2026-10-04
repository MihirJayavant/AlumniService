using System.Text.Json;
using Alumni.Api.Controllers;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OneOf;
using Xunit;

namespace Alumni.Api.UnitTests.Controllers;

public class ResultHelperTests
{
    [Fact]
    public void ToServerResult_WhenSuccessful_ReturnsOkWithOriginalPayload()
    {
        var payload = new PaginatedList<string>
        {
            Items = ["first", "second"],
            TotalCount = 5,
            PageNumber = 2,
            PageSize = 2
        };
        OneOf<PaginatedList<string>, ErrorType> response = payload;

        var result = response.ToServerResult();

        Assert.Equal(200, Assert.IsType<IStatusCodeHttpResult>(result, exactMatch: false).StatusCode);
        Assert.Same(payload, Assert.IsType<IValueHttpResult>(result, exactMatch: false).Value);
    }

    [Theory]
    [InlineData(ResponseStatus.BadRequest, 400)]
    [InlineData(ResponseStatus.NotFound, 404)]
    [InlineData(ResponseStatus.Conflict, 409)]
    [InlineData(ResponseStatus.Forbidden, 403)]
    [InlineData(ResponseStatus.TooManyRequests, 429)]
    [InlineData((ResponseStatus)499, 499)]
    [InlineData(ResponseStatus.NotImplemented, 501)]
    [InlineData(ResponseStatus.ServiceUnavailable, 503)]
    [InlineData((ResponseStatus)599, 599)]
    public void ToServerResult_WhenSupportedError_PreservesStatusAndMessage(ResponseStatus status, int expectedStatus)
    {
        OneOf<string, ErrorType> response = new ErrorType { Status = status, Message = "Specific failure" };

        var result = response.ToServerResult();

        Assert.Equal(expectedStatus, Assert.IsType<IStatusCodeHttpResult>(result, exactMatch: false).StatusCode);
        var value = Assert.IsType<IValueHttpResult>(result, exactMatch: false).Value;
        var body = JsonSerializer.SerializeToElement(value);
        Assert.Equal("Specific failure", body.GetProperty("Error").GetString());
    }

    [Fact]
    public async Task ToServerResult_WhenUnauthorized_ReturnsStandard401WithoutBody()
    {
        OneOf<string, ErrorType> response = new ErrorType
        {
            Status = ResponseStatus.Unauthorized,
            Message = "Authentication failed"
        };

        var result = response.ToServerResult();

        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
        Assert.Equal(401, Assert.IsType<IStatusCodeHttpResult>(result, exactMatch: false).StatusCode);
    }

    [Fact]
    public void ToServerResult_WhenInternalError_ReturnsProblemDetails()
    {
        OneOf<string, ErrorType> response = new ErrorType { Message = "Operation failed" };

        var result = response.ToServerResult();

        Assert.Equal(500, Assert.IsType<IStatusCodeHttpResult>(result, exactMatch: false).StatusCode);
        var problem = Assert.IsType<ProblemDetails>(Assert.IsType<IValueHttpResult>(result, exactMatch: false).Value);
        Assert.Equal(500, problem.Status);
        Assert.Equal("Operation failed", problem.Detail);
    }

    [Theory]
    [InlineData((ResponseStatus)(-1))]
    [InlineData((ResponseStatus)0)]
    [InlineData(ResponseStatus.Continue)]
    [InlineData(ResponseStatus.Success)]
    [InlineData(ResponseStatus.NoContent)]
    [InlineData(ResponseStatus.TemporaryRedirect)]
    [InlineData((ResponseStatus)399)]
    [InlineData((ResponseStatus)600)]
    public void ToServerResult_WhenStatusIsOutsideErrorRange_ReturnsInternalErrorProblem(ResponseStatus status)
    {
        OneOf<string, ErrorType> response = new ErrorType { Status = status, Message = "Invalid error status" };

        var result = response.ToServerResult();

        Assert.Equal(500, Assert.IsType<IStatusCodeHttpResult>(result, exactMatch: false).StatusCode);
        var problem = Assert.IsType<ProblemDetails>(Assert.IsType<IValueHttpResult>(result, exactMatch: false).Value);
        Assert.Equal(500, problem.Status);
        Assert.Equal("Invalid error status", problem.Detail);
    }
}
