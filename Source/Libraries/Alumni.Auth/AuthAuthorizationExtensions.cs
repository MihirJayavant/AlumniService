using Microsoft.Extensions.DependencyInjection;

namespace Alumni.Auth;

public static class AuthAuthorizationExtensions
{
    public static IServiceCollection AddAuthAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPermissions.StudentsRead, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.Student, AuthRoles.FacultyReader, AuthRoles.FacultyEditor, AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.StudentsReadAll, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.FacultyReader, AuthRoles.FacultyEditor, AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.StudentsWrite, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.FacultyEditor, AuthRoles.FacultyAdmin));

            options.AddPolicy(AuthPermissions.StudentsSelfService, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.Student));

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
