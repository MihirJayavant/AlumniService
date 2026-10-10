using Alumni.Api.Services;
using Alumni.Auth;
using Alumni.Auth.Bootstrap;
using Alumni.Auth.Invitations;
using Alumni.Faculty;
using Alumni.Student;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class InvitationRegistrationTests
{
    [Fact]
    public void AddInfrastructureServices_WhenResolved_SharesContextAndInvitationServiceWithinScope()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationContext>();

        Assert.Same(context, services.GetRequiredService<IApplicationContext>());
        Assert.Same(context, services.GetRequiredService<IStudentDbContext>());
        Assert.Same(context, services.GetRequiredService<IFacultyDbContext>());
        Assert.Same(services.GetRequiredService<InvitationService>(), services.GetRequiredService<IInvitationService>());
        Assert.Same(services.GetRequiredService<IInvitationService>(), services.GetRequiredService<IInvitationProvisioningService>());
        Assert.NotNull(services.GetRequiredService<UserManager<AuthUser>>());
        Assert.IsType<AdminBootstrapStore>(services.GetRequiredService<IAdminBootstrapStore>());
        Assert.NotNull(services.GetRequiredService<BootstrapAdminHandler>());
        Assert.IsAssignableFrom<IUserPasswordStore<AuthUser>>(services.GetRequiredService<IUserStore<AuthUser>>());
        Assert.IsAssignableFrom<IUserRoleStore<AuthUser>>(services.GetRequiredService<IUserStore<AuthUser>>());
    }

    [Fact]
    public void AddInfrastructureServices_WhenConfigured_RequiresUniqueEmailAndTwelveCharacterPasswords()
    {
        using var provider = CreateServices();

        var options = provider.GetRequiredService<IOptions<IdentityOptions>>().Value;

        Assert.True(options.User.RequireUniqueEmail);
        Assert.Equal(12, options.Password.RequiredLength);
    }

    private static ServiceProvider CreateServices()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Environment"] = "Development",
            ["ConnectionStrings:alumni-db"] = "Host=localhost;Database=registration-tests;Username=test"
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructureServices(new SettingService(configuration));
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }
}
