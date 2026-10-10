using Alumni.Api.Controllers;
using Alumni.Faculty;
using Alumni.Student;
using Alumni.Student.FurtherStudy;
using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alumni.Api.UnitTests.Controllers;

public class EndpointRegistrationTests
{
    [Fact]
    public async Task AddControllers_WhenRegistered_ExposesAllRoutesExactlyOnce()
    {
        await using var app = CreateApplication();
        app.AddControllers();

        var routes = GetEndpoints(app)
            .SelectMany(endpoint => endpoint.Metadata.GetRequiredMetadata<HttpMethodMetadata>().HttpMethods
                .Select(method => $"{method} {endpoint.RoutePattern.RawText}"))
            .Order(StringComparer.Ordinal).ToArray();
        string[] expected =
        [
            "GET /student/", "GET /student/{id:guid}", "POST /student/",
            "GET /faculty/", "GET /faculty/{facultyId:guid}", "POST /faculty/", "DELETE /faculty/{facultyId:guid}",
            "GET /company/{studentId:guid}", "POST /company/",
            "GET /exam/{studentId:guid}", "POST /exam/",
            "GET /further-studies/{studentId:guid}", "POST /further-studies/",
            "POST /auth/invitations/accept", "POST /auth/bootstrap"
        ];

        Assert.Equal(expected.Order(StringComparer.Ordinal), routes);
    }

    [Fact]
    public async Task Add_WhenFurtherStudyGetRegistered_AdvertisesPaginatedResponse()
    {
        await using var app = CreateApplication();
        new FurtherStudiesController().Add(app);

        var endpoint = Assert.Single(GetEndpoints(app), endpoint =>
            endpoint.Metadata.GetRequiredMetadata<HttpMethodMetadata>().HttpMethods.Contains("GET"));
        var response = Assert.Single(endpoint.Metadata.GetOrderedMetadata<IProducesResponseTypeMetadata>(),
            metadata => metadata.StatusCode == 200);

        Assert.Equal(typeof(PaginatedList<FurtherStudyResponse>), response.Type);
    }

    private static WebApplication CreateApplication()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [] });
        builder.Services.AddScoped<IStudentDbContext>(_ => throw new InvalidOperationException("Database must not be accessed."));
        builder.Services.AddScoped<IFacultyDbContext>(_ => throw new InvalidOperationException("Database must not be accessed."));
        return builder.Build();
    }

    private static IEnumerable<RouteEndpoint> GetEndpoints(WebApplication app)
        => ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints).OfType<RouteEndpoint>();
}
