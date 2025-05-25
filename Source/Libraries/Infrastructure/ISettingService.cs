namespace Infrastructure;

public interface ISettingService
{
    string Environment { get; init; }
    DatabaseSetting DatabaseSetting { get; init; }
    AuthSetting AuthSetting { get; init; }
    bool IsDevelopment { get; }
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
