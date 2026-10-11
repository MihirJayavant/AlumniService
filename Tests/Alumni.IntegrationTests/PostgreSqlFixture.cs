using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;
using Xunit;

namespace Alumni.IntegrationTests;

[CollectionDefinition("PostgreSQL", DisableParallelization = true)]
public sealed class PostgreSqlTestGroup : ICollectionFixture<PostgreSqlFixture>;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer database = new PostgreSqlBuilder("postgres:17.6-alpine")
        .WithDatabase("alumni_integration_tests")
        .Build();

    private Respawner respawner = null!;

    public ApplicationContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationContext>()
        .UseNpgsql(database.GetConnectionString())
        .Options);

    public async ValueTask InitializeAsync()
    {
        await database.StartAsync();
        await using var context = CreateContext();
        // The checked-in migrations predate the current model. This suite validates
        // current-model persistence; migration upgrade coverage remains separate.
        await context.Database.EnsureCreatedAsync();

        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();
        respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["Faculty", "Student", "Auth", "public"],
            TablesToIgnore = [new Table("public", "__EFMigrationsHistory")],
        });
    }

    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        await connection.OpenAsync();
        await respawner.ResetAsync(connection);
    }

    public ValueTask DisposeAsync() => database.DisposeAsync();
}
