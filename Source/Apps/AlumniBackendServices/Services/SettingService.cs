namespace AlumniBackendServices.Services;

public sealed class SettingService : ISettingService
{
    public string Environment { get; init; }
    public DatabaseSetting DatabaseSetting { get; init; }
    public AuthSetting AuthSetting { get; init; }

    public bool IsDevelopment => Environment == "Development";

    public SettingService(IConfiguration configuration)
    {
        Environment = configuration[nameof(Environment)] ?? "Development";
        var pass = IsDevelopment ? configuration["Database:Password"] : System.Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        DatabaseSetting = new()
        {
            Connection = configuration["Database:Connection"] ?? "",
            Password = pass ?? ""
        };

        AuthSetting = new()
        {
            Secret = configuration["Authentication:Secret"] ?? "",
            ValidAudience = configuration["Authentication:ValidAudience"] ?? "",
            ValidIssuer = configuration["Authentication:ValidIssuer"] ?? ""
        };
    }
}
