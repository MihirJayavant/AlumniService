using Alumni.Api.Services;
using Alumni.Auth;

namespace Alumni.Api.ExtensionService;

public static class AuthorizationExtension
{
    public static IServiceCollection AddApplicationAuthorization(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentActor, CurrentActor>();
        services.AddAuthAuthorization();

        return services;
    }
}
