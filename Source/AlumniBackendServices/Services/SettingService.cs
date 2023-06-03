using Application;
using NuGet.Common;

namespace AlumniBackendServices.Services;

public class SettingService : ISettingService
{
    public string Environment { get; init; }
    public DatabaseSetting DatabaseSetting { get; init; }
    public string TokenSecret { get; init; }

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
        TokenSecret = configuration["Authentication:Secret"] ?? "";
    }
}
