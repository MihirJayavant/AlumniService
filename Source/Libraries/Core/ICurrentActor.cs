namespace Core;

/// <summary>Identifies the caller independently of the API transport.</summary>
public interface ICurrentActor
{
    public bool IsAuthenticated { get; }

    /// <summary>The authenticated account ID, or null when no authenticated subject is available.</summary>
    public string? UserId { get; }
}
