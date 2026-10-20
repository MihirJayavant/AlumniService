using Microsoft.Extensions.DependencyInjection;

namespace Alumni.Auth;

public static class AuthAuthorizationExtensions
{
    public static IServiceCollection AddAuthAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPermissions.MembersRead, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.Member, AuthRoles.FacultyReader, AuthRoles.FacultyEditor, AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.MembersReadAll, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.FacultyReader, AuthRoles.FacultyEditor, AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.MembersWrite, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.FacultyEditor, AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.MembersSelfService, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.Member));

            options.AddPolicy(AuthPermissions.FacultyManage, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.InvitationsManage, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.FacultyAdmin));
        });

        return services;
    }
}
