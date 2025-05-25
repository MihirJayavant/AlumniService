namespace AlumniBackendServices.ExtensionService;

public static class AuthExtension
{
    public static void AddAuth(this IServiceCollection services) { }

    public static void UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
