using Application.Common.Models;

namespace Application.Contracts.Response;

public class Response<T>
{
    public T Result { get; set; } = default!;
    public ResponseStatus Status { get; set; }
    public ErrorResponse Error { get; set; } = default!;
}
