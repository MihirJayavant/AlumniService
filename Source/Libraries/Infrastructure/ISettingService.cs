namespace Infrastructure;

public interface ISettingService
{
    public string Environment { get; init; }
    public DatabaseSetting DatabaseSetting { get; init; }
    public AuthSetting AuthSetting { get; init; }
    public bool IsDevelopment { get; }
}

public record AuthSetting
{
    public required string Secret { get; init; }
    public required string ValidAudience { get; init; }
    public required string ValidIssuer { get; init; }
}

public record DatabaseSetting
{
    public required string Connection { get; init; }
    public required string Password { get; init; }
}
