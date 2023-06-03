namespace Application;

public interface ISettingService
{
    string Environment { get; init; }
    DatabaseSetting DatabaseSetting { get; init; }
    string TokenSecret { get; init; }
    bool IsDevelopment { get; }
}

public record DatabaseSetting
{
    public required string Connection { get; init; }
    public required string Password { get; init; }
}
