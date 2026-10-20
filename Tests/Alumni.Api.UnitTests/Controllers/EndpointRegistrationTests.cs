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
            "GET /members/", "GET /members/{id:guid}", "POST /members/",
            "GET /faculty/", "GET /faculty/{facultyId:guid}", "POST /faculty/", "DELETE /faculty/{facultyId:guid}",
            "GET /company/{memberId:guid}", "POST /company/",
            "GET /exam/{memberId:guid}", "POST /exam/",
            "GET /further-studies/{memberId:guid}", "POST /further-studies/",
            "POST /auth/bootstrap"
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
        HandlerTestRegistration.AddUnreachableHandlers(builder.Services);
        return builder.Build();
    }

    private static IEnumerable<RouteEndpoint> GetEndpoints(WebApplication app)
        => ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints).OfType<RouteEndpoint>();
}
