using Alumni.Api.Services;
using Alumni.Auth;
using Alumni.Faculty;
using Alumni.Student;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public class InvitationRegistrationTests
{
    [Fact]
    public void AddInfrastructureServices_WhenResolved_SharesContextAndResolvesBootstrapHandlerWithinScope()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationContext>();

        Assert.Same(context, services.GetRequiredService<IApplicationContext>());
        Assert.Same(context, services.GetRequiredService<IStudentDbContext>());
        Assert.Same(context, services.GetRequiredService<IFacultyDbContext>());
        Assert.Same(context, services.GetRequiredService<IAuthDbContext>());
        Assert.NotNull(services.GetRequiredService<UserManager<AuthUser>>());
        Assert.Same(services.GetRequiredService<BootstrapAdminHandler>(),
            services.GetRequiredService<BootstrapAdminHandler>());
        Assert.NotNull(services.GetRequiredService<TimeProvider>());
        Assert.IsAssignableFrom<IUserPasswordStore<AuthUser>>(services.GetRequiredService<IUserStore<AuthUser>>());
        Assert.IsAssignableFrom<IUserRoleStore<AuthUser>>(services.GetRequiredService<IUserStore<AuthUser>>());

        using var otherScope = provider.CreateScope();
        Assert.NotSame(context, otherScope.ServiceProvider.GetRequiredService<IAuthDbContext>());
        Assert.NotSame(services.GetRequiredService<BootstrapAdminHandler>(),
            otherScope.ServiceProvider.GetRequiredService<BootstrapAdminHandler>());
    }

    [Fact]
    public void AddInfrastructureServices_WhenConfigured_RequiresUniqueEmailAndEightCharacterPasswords()
    {
        using var provider = CreateServices();

        var options = provider.GetRequiredService<IOptions<IdentityOptions>>().Value;

        Assert.True(options.User.RequireUniqueEmail);
        Assert.Equal(8, options.Password.RequiredLength);
    }

    public static TheoryData<Type> HandlerTypes
    {
        get
        {
            var handlers = new TheoryData<Type>();
            foreach (var handler in HandlerTestRegistration.AllHandlers)
            {
                handlers.Add(handler);
            }

            return handlers;
        }
    }

    [Theory]
    [MemberData(nameof(HandlerTypes))]
    public void AddInfrastructureServices_WhenHandlerResolved_ReusesWithinScopeAndSeparatesScopes(Type handlerType)
    {
        using var provider = CreateServices();
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        var handler = firstScope.ServiceProvider.GetRequiredService(handlerType);

        Assert.Same(handler, firstScope.ServiceProvider.GetRequiredService(handlerType));
        Assert.NotSame(handler, secondScope.ServiceProvider.GetRequiredService(handlerType));
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
