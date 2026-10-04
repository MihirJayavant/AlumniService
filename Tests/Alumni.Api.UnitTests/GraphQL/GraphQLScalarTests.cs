using HotChocolate.Types;
using Xunit;

namespace Alumni.Api.UnitTests.GraphQL;

public class GraphQLScalarTests
{
    [Fact]
    public async Task Execute_WhenSerializingBoundScalars_PreservesGuidDateLongAndNullableTimestamp()
    {
        await using var context = new GraphQLTestContext(includeScalarFixture: true);

        var result = await context.ExecuteAsync(
            "{ scalarContract { identifier birthDate salary createdAt updatedAt } }");

        Assert.False(result.TryGetProperty("errors", out _), result.ToString());
        var data = result.GetProperty("data").GetProperty("scalarContract");
        Assert.Equal(Guid.Parse(GraphQLTestContext.ValidId), Guid.Parse(data.GetProperty("identifier").GetString()!));
        Assert.Equal("1990-01-02", data.GetProperty("birthDate").GetString());
        Assert.Equal(5_000_000_000L, data.GetProperty("salary").GetInt64());
        Assert.Equal(new DateTime(2020, 3, 4, 5, 6, 7, DateTimeKind.Utc), data.GetProperty("createdAt").GetDateTime());
        Assert.Equal(System.Text.Json.JsonValueKind.Null, data.GetProperty("updatedAt").ValueKind);
        Assert.Empty(context.Accesses);
    }
}

public sealed class GraphQLScalarFixtureQuery : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name("Query");
        descriptor.Field("scalarContract")
            .Type<ObjectType<ScalarContract>>()
            .Resolve(_ => new ScalarContract(
                Guid.Parse(GraphQLTestContext.ValidId),
                new DateOnly(1990, 1, 2),
                5_000_000_000L,
                new DateTime(2020, 3, 4, 5, 6, 7, DateTimeKind.Utc),
                null));
    }
}

public sealed record ScalarContract(Guid Identifier, DateOnly BirthDate, long Salary, DateTime CreatedAt, DateTime? UpdatedAt);
