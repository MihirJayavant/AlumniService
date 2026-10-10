using Alumni.Auth;
using Alumni.Student;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenIddict.EntityFrameworkCore.Models;
using Xunit;

namespace Alumni.Api.UnitTests.Auth;

public sealed class AuthModelTests
{
    [Fact]
    public void Model_WhenConstructed_KeepsAuthTablesSeparateFromDomainSchemas()
    {
        using var context = CreateContext();
        var model = context.Model;
        Type[] authTypes =
        [
            typeof(AuthUser), typeof(AuthRole), typeof(IdentityUserRole<string>),
            typeof(IdentityUserClaim<string>), typeof(IdentityUserLogin<string>),
            typeof(IdentityUserToken<string>), typeof(IdentityRoleClaim<string>),
            typeof(OpenIddictEntityFrameworkCoreApplication),
            typeof(OpenIddictEntityFrameworkCoreAuthorization),
            typeof(OpenIddictEntityFrameworkCoreScope), typeof(OpenIddictEntityFrameworkCoreToken),
            typeof(Invitation), typeof(AuthSession)
        ];

        Assert.All(authTypes, type => Assert.Equal("Auth", Entity(model, type).GetSchema()));
        Assert.Equal("Student", Entity(model, typeof(StudentEntity)).GetSchema());
        Assert.Equal("Faculty", Entity(model, typeof(Alumni.Faculty.Faculty)).GetSchema());
        Assert.NotEmpty(model.GetRelationalModel().Tables);
    }

    [Fact]
    public void Model_WhenEmailIdentifiesAnAccount_RequiresUniqueNormalizedEmail()
    {
        using var context = CreateContext();
        var user = Entity(context.Model, typeof(AuthUser));

        Assert.False(user.FindProperty(nameof(AuthUser.Email))!.IsNullable);
        Assert.False(user.FindProperty(nameof(AuthUser.NormalizedEmail))!.IsNullable);
        Assert.True(Assert.Single(user.GetIndexes(), index =>
            index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(AuthUser.NormalizedEmail)])).IsUnique);
    }

    [Theory]
    [InlineData(typeof(StudentEntity))]
    [InlineData(typeof(Alumni.Faculty.Faculty))]
    public void Model_WhenProfileReferencesAnAccount_UsesUniqueIdentifierWithoutForeignKey(Type profileType)
    {
        using var context = CreateContext();
        var profile = Entity(context.Model, profileType);
        const string propertyName = "AuthUserId";

        Assert.NotNull(profile.FindProperty(propertyName));
        Assert.DoesNotContain(profile.GetForeignKeys(), foreignKey =>
            foreignKey.Properties.Any(property => property.Name == propertyName));
        Assert.True(Assert.Single(profile.GetIndexes(), index =>
            index.Properties.Select(property => property.Name).SequenceEqual([propertyName])).IsUnique);
        Assert.DoesNotContain(context.Model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()),
            foreignKey =>
            {
                var belongsToAuth = foreignKey.DeclaringEntityType.GetSchema() == "Auth";
                var referencesAuth = foreignKey.PrincipalEntityType.GetSchema() == "Auth";
                return belongsToAuth != referencesAuth;
            });
    }

    [Fact]
    public void Model_WhenInvitingAnExistingAccount_ReferencesTheAccountWithoutDomainProfiles()
    {
        using var context = CreateContext();
        var invitation = Entity(context.Model, typeof(Invitation));
        var target = Assert.Single(invitation.GetForeignKeys(), foreignKey =>
            foreignKey.Properties.Select(property => property.Name).SequenceEqual([nameof(Invitation.UserId)]));

        Assert.Equal(typeof(AuthUser), target.PrincipalEntityType.ClrType);
        Assert.True(target.IsRequired);
        var account = Entity(context.GetService<IDesignTimeModel>().Model, typeof(AuthUser));
        Assert.Equal(AccountStatus.PendingActivation, account.FindProperty(nameof(AuthUser.Status))!.GetDefaultValue());
        Assert.Contains(account.GetCheckConstraints(), constraint => constraint.Name == "CK_Users_ActivePassword");
    }

    [Fact]
    public void Model_WhenSeedingRoles_ProvidesOnlyTheFourAgreedRoles()
    {
        using var context = CreateContext();
        var model = context.GetService<IDesignTimeModel>().Model;
        var roles = Entity(model, typeof(AuthRole)).GetSeedData().ToArray();
        string[] expectedRoles = ["Student", "FacultyReader", "FacultyEditor", "FacultyAdmin"];

        Assert.Equal(expectedRoles.Order(), roles.Select(role => (string)role[nameof(AuthRole.Name)]!).Order());
        Assert.All(roles, role =>
            Assert.Equal(((string)role[nameof(AuthRole.Name)]!).ToUpperInvariant(),
                role[nameof(AuthRole.NormalizedName)]));
        Assert.Equal(4, roles.Select(role => role[nameof(AuthRole.Id)]).Distinct().Count());
    }

    [Fact]
    public void Model_WhenInvitationsAndSessionsChange_SupportsConcurrencyAndSessionGrantLinkage()
    {
        using var context = CreateContext();
        var invitation = Entity(context.Model, typeof(Invitation));
        var session = Entity(context.Model, typeof(AuthSession));

        Assert.All(new[]
        {
            invitation.FindProperty(nameof(Invitation.Version))!,
            session.FindProperty(nameof(AuthSession.Version))!
        }, version =>
        {
            Assert.True(version.IsConcurrencyToken);
            Assert.Equal(ValueGenerated.OnAddOrUpdate, version.ValueGenerated);
            Assert.Equal("xmin", version.GetColumnName());
        });
        var grant = Assert.Single(session.GetForeignKeys(), foreignKey =>
            foreignKey.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(AuthSession.AuthorizationId)]));
        Assert.Equal(typeof(OpenIddictEntityFrameworkCoreAuthorization), grant.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.SetNull, grant.DeleteBehavior);
        Assert.True(Assert.Single(session.GetIndexes(), index =>
            index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(AuthSession.AuthorizationId)])).IsUnique);
    }

    private static ApplicationContext CreateContext() => new(
        new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql("Host=localhost;Database=auth_model_tests;Username=model_tests")
            .Options);

    private static IEntityType Entity(IModel model, Type type) =>
        Assert.IsAssignableFrom<IEntityType>(model.FindEntityType(type));
}
