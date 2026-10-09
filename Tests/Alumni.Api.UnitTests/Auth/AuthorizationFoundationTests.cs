using System.Security.Claims;
using Alumni.Api.ExtensionService;
using Alumni.Auth;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public sealed class AuthorizationFoundationTests
{
    public static TheoryData<string, string, bool> RolePermissions
    {
        get
        {
            var data = new TheoryData<string, string, bool>();
            (string Policy, string[] Roles)[] permissions =
            [
                (AuthPermissions.StudentsRead, ["Student", "FacultyReader", "FacultyEditor", "FacultyAdmin"]),
                (AuthPermissions.StudentsReadAll, ["FacultyReader", "FacultyEditor", "FacultyAdmin"]),
                (AuthPermissions.StudentsWrite, ["FacultyEditor", "FacultyAdmin"]),
                (AuthPermissions.StudentsSelfService, ["Student"]),
                (AuthPermissions.FacultyManage, ["FacultyAdmin"]),
                (AuthPermissions.InvitationsManage, ["FacultyAdmin"])
            ];

            foreach (var (policy, permittedRoles) in permissions)
            {
                foreach (var role in new[] { "Student", "FacultyReader", "FacultyEditor", "FacultyAdmin", "Unknown", "" })
                {
                    data.Add(policy, role, permittedRoles.Contains(role));
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(RolePermissions))]
    public async Task AuthorizeAsync_WhenAuthenticated_EnforcesPermissionForRole(
        string policy, string role, bool expectedSuccess)
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var authorization = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        Claim[] claims = role.Length == 0 ? [] : [new Claim(ClaimTypes.Role, role)];
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

        var result = await authorization.AuthorizeAsync(principal, resource: null, policy);

        Assert.Equal(expectedSuccess, result.Succeeded);
    }

    [Theory]
    [InlineData(AuthPermissions.StudentsRead, "Student")]
    [InlineData(AuthPermissions.StudentsReadAll, "FacultyReader")]
    [InlineData(AuthPermissions.StudentsWrite, "FacultyEditor")]
    [InlineData(AuthPermissions.StudentsSelfService, "Student")]
    [InlineData(AuthPermissions.FacultyManage, "FacultyAdmin")]
    [InlineData(AuthPermissions.InvitationsManage, "FacultyAdmin")]
    public async Task AuthorizeAsync_WhenAnonymousWithPermittedRole_RejectsCaller(string policy, string role)
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var authorization = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, role)]));

        var result = await authorization.AuthorizeAsync(principal, resource: null, policy);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void CurrentActor_WhenContextIsAbsent_HasNoAuthenticatedAccount()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var actor = scope.ServiceProvider.GetRequiredService<ICurrentActor>();

        Assert.False(actor.IsAuthenticated);
        Assert.Null(actor.UserId);
    }

    [Fact]
    public void CurrentActor_WhenAnonymousSubjectIsPresent_IgnoresSubject()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        SetPrincipal(provider, new ClaimsIdentity([new Claim(AuthClaimTypes.Subject, "anonymous-account")]));
        var actor = scope.ServiceProvider.GetRequiredService<ICurrentActor>();

        Assert.False(actor.IsAuthenticated);
        Assert.Null(actor.UserId);
    }

    [Fact]
    public void CurrentActor_WhenAuthenticated_ReturnsAccountSubject()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        SetPrincipal(provider, new ClaimsIdentity([new Claim(AuthClaimTypes.Subject, "account-123")], "test"));
        var actor = scope.ServiceProvider.GetRequiredService<ICurrentActor>();

        Assert.True(actor.IsAuthenticated);
        Assert.Equal("account-123", actor.UserId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CurrentActor_WhenAuthenticatedWithoutUsableSubject_HasNoAccountId(string? subject)
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        Claim[] claims = subject is null ? [] : [new Claim(AuthClaimTypes.Subject, subject)];
        SetPrincipal(provider, new ClaimsIdentity(claims, "test"));
        var actor = scope.ServiceProvider.GetRequiredService<ICurrentActor>();

        Assert.True(actor.IsAuthenticated);
        Assert.Null(actor.UserId);
    }

    [Fact]
    public void CurrentActor_WhenAnonymousIdentityPrecedesAuthenticatedIdentity_UsesAuthenticatedSubject()
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        SetPrincipal(provider,
            new ClaimsIdentity([new Claim(AuthClaimTypes.Subject, "untrusted-account")]),
            new ClaimsIdentity([new Claim(AuthClaimTypes.Subject, "account-123")], "test"));
        var actor = scope.ServiceProvider.GetRequiredService<ICurrentActor>();

        Assert.True(actor.IsAuthenticated);
        Assert.Equal("account-123", actor.UserId);
    }

    [Fact]
    public void CurrentActor_WhenResolvedInResolverScope_UsesSharedRequestPrincipal()
    {
        using var provider = CreateServices();
        using var requestScope = provider.CreateScope();
        SetPrincipal(provider, new ClaimsIdentity([new Claim(AuthClaimTypes.Subject, "account-123")], "test"));
        var requestActor = requestScope.ServiceProvider.GetRequiredService<ICurrentActor>();
        using var resolverScope = requestScope.ServiceProvider.CreateScope();
        var resolverActor = resolverScope.ServiceProvider.GetRequiredService<ICurrentActor>();

        Assert.NotSame(requestActor, resolverActor);
        Assert.True(resolverActor.IsAuthenticated);
        Assert.Equal("account-123", resolverActor.UserId);
        Assert.Equal(requestActor.UserId, resolverActor.UserId);
        Assert.Same(requestScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>(),
            resolverScope.ServiceProvider.GetRequiredService<IHttpContextAccessor>());
    }

    private static ServiceProvider CreateServices()
    {
        var provider = new ServiceCollection()
            .AddLogging()
            .AddApplicationAuthorization()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        provider.GetRequiredService<IHttpContextAccessor>().HttpContext = null;
        return provider;
    }

    private static void SetPrincipal(IServiceProvider provider, params ClaimsIdentity[] identities) =>
        provider.GetRequiredService<IHttpContextAccessor>().HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identities)
        };
}
