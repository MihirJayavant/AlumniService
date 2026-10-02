namespace AlumniBackendServices.ExtensionService;

public static class AuthExtension
{
    extension(IServiceCollection services)
    {
        public static void AddAuth() { }
    }

    extension(IApplicationBuilder app)
    {
        public void UseAuth()
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
