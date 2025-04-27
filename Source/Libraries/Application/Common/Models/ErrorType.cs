namespace Application.Common.Models;

public sealed record ErrorType(ResponseStatus Status, string Message);
