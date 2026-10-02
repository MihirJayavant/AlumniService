namespace Core;

public sealed record ErrorType{
    public required string Message { get; init; }
    public ResponseStatus Status { get; init; } = ResponseStatus.InternalError;
};
