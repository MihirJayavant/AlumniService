using Alumni.Api.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Alumni.Api.UnitTests.Services;

[CollectionDefinition("Settings environment", DisableParallelization = true)]
public sealed class SettingsEnvironmentDefinition
{
}

[Collection("Settings environment")]
public sealed class SettingServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("Development")]
    public void Constructor_WhenEnvironmentIsDevelopmentOrMissing_UsesConfiguredPassword(string? environment)
        => WithDatabasePassword("environment-password", () =>
        {
            var settings = CreateSettings(new()
            {
                ["Environment"] = environment,
                ["Database:Password"] = "configured-password",
                ["Database:Connection"] = "Host=localhost;Password={0}"
            });

            Assert.Equal("Development", settings.Environment);
            Assert.True(settings.IsDevelopment);
            Assert.Equal("configured-password", settings.DatabaseSetting.Password);
            Assert.Equal("Host=localhost;Password=configured-password", settings.DatabaseSetting.Connection);
        });

    [Fact]
    public void Constructor_WhenCompleteConnectionStringIsSupplied_PreservesItAndIgnoresLegacyTemplate()
    {
        const string connection = " Host=localhost;Database=alumni;Password=connection-password ";
        var settings = CreateSettings(new()
        {
            ["ConnectionStrings:alumni-db"] = connection,
            ["Database:Connection"] = "malformed { template",
            ["Database:Password"] = "configured-password"
        });

        Assert.Equal(connection, settings.DatabaseSetting.Connection);
        Assert.Equal("configured-password", settings.DatabaseSetting.Password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    public void Constructor_WhenCompleteConnectionStringIsBlank_FormatsLegacyTemplate(string? connection)
    {
        var settings = CreateSettings(new()
        {
            ["ConnectionStrings:alumni-db"] = connection,
            ["Database:Connection"] = "Host=localhost;Password={0}",
            ["Database:Password"] = "configured-password"
        });

        Assert.Equal("Host=localhost;Password=configured-password", settings.DatabaseSetting.Connection);
    }

    [Fact]
    public void Constructor_WhenConfigurationIsMissing_DefaultsEnvironmentAndSettings()
    {
        var settings = CreateSettings(new());

        Assert.Equal("Development", settings.Environment);
        Assert.True(settings.IsDevelopment);
        Assert.Equal("", settings.DatabaseSetting.Connection);
        Assert.Equal("", settings.DatabaseSetting.Password);
        Assert.Equal("", settings.AuthSetting.Secret);
        Assert.Equal("", settings.AuthSetting.ValidAudience);
        Assert.Equal("", settings.AuthSetting.ValidIssuer);
    }

    [Fact]
    public void Constructor_WhenDevelopmentPasswordIsMissing_SubstitutesEmptyPassword()
    {
        var settings = CreateSettings(new()
        {
            ["Database:Connection"] = "Host=localhost;Password={0}"
        });

        Assert.Equal("", settings.DatabaseSetting.Password);
        Assert.Equal("Host=localhost;Password=", settings.DatabaseSetting.Connection);
    }

    [Fact]
    public void Constructor_WhenAuthenticationSettingsAreSupplied_PreservesTheirValues()
    {
        var settings = CreateSettings(new()
        {
            ["Authentication:Secret"] = "test-signing-value",
            ["Authentication:ValidAudience"] = "test-audience",
            ["Authentication:ValidIssuer"] = "test-issuer"
        });

        Assert.Equal("test-signing-value", settings.AuthSetting.Secret);
        Assert.Equal("test-audience", settings.AuthSetting.ValidAudience);
        Assert.Equal("test-issuer", settings.AuthSetting.ValidIssuer);
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public void Constructor_WhenEnvironmentIsNotDevelopment_UsesEnvironmentPassword(string environment)
        => WithDatabasePassword("environment-password", () =>
        {
            var settings = CreateSettings(new()
            {
                ["Environment"] = environment,
                ["Database:Password"] = "configured-password",
                ["Database:Connection"] = "Host=localhost;Password={0}"
            });

            Assert.Equal(environment, settings.Environment);
            Assert.False(settings.IsDevelopment);
            Assert.Equal("environment-password", settings.DatabaseSetting.Password);
            Assert.Equal("Host=localhost;Password=environment-password", settings.DatabaseSetting.Connection);
        });

    [Fact]
    public void Constructor_WhenProductionEnvironmentPasswordIsMissing_SubstitutesEmptyPassword()
        => WithDatabasePassword(null, () =>
        {
            var settings = CreateSettings(new()
            {
                ["Environment"] = "Production",
                ["Database:Password"] = "configured-password",
                ["Database:Connection"] = "Host=localhost;Password={0}"
            });

            Assert.Equal("", settings.DatabaseSetting.Password);
            Assert.Equal("Host=localhost;Password=", settings.DatabaseSetting.Connection);
        });

    private static SettingService CreateSettings(Dictionary<string, string?> values)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        return new SettingService(configuration);
    }

    private static void WithDatabasePassword(string? password, Action assertions)
    {
        var originalPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        try
        {
            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", password);
            assertions();
        }
        finally
        {
            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", originalPassword);
        }
    }
}
