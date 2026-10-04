using Alumni.Api.ExtensionService;
using Alumni.Faculty;
using Alumni.Student;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alumni.Api.UnitTests.GraphQL;

public class GraphQLEndpointTests
{
    [Fact]
    public async Task MapApplicationGraphQL_WhenRegistered_ExposesGraphQLEndpointWithoutDatabaseAccess()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [] });
        builder.Services.AddScoped<IStudentDbContext>(_ => throw new InvalidOperationException("Database must not be accessed."));
        builder.Services.AddScoped<IFacultyDbContext>(_ => throw new InvalidOperationException("Database must not be accessed."));
        builder.Services.AddApplicationGraphQL();
        await using var app = builder.Build();

        app.MapApplicationGraphQL();

        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints).OfType<RouteEndpoint>();
        var endpoint = Assert.Single(endpoints, candidate =>
            Matches(candidate, "/graphql"));
        Assert.False(Matches(endpoint, "/unrelated"));
    }

    private static bool Matches(RouteEndpoint endpoint, string path)
    {
        var template = TemplateParser.Parse(endpoint.RoutePattern.RawText!);
        var matcher = new TemplateMatcher(template, new RouteValueDictionary(endpoint.RoutePattern.Defaults));
        return matcher.TryMatch(new PathString(path), new RouteValueDictionary());
    }
}
