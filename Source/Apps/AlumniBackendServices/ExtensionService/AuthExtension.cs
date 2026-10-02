namespace AlumniBackendServices.ExtensionService;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1708:Identifiers should differ by more than case",
    Justification = "False positive for multiple C# extension blocks: https://github.com/dotnet/sdk/issues/51716")]
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
