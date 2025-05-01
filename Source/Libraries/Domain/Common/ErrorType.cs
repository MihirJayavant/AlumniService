namespace Domain.Common;

public sealed record ErrorType(string Message, ResponseStatus Status = ResponseStatus.InternalError);
