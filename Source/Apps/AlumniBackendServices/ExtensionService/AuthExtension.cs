using AlumniBackendServices.Services;
using Application.Common.Interfaces;

namespace AlumniBackendServices.ExtensionService;

public static class AuthExtension
{
    public static void AddAuth(this IServiceCollection services) => services.AddSingleton<ICurrentUserService, CurrentUserService>();

    public static void UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
