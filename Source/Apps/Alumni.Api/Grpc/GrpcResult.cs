using Grpc.Core;

namespace Alumni.Api.Grpc;

internal static class GrpcResult
{
    public static TReply Map<T, TReply>(OneOf<T, ErrorType> result, Func<T, TReply> mapper)
        => result.Match(mapper, error => throw ToException(error));

    private static RpcException ToException(ErrorType error)
    {
        var status = error.Status switch
        {
            ResponseStatus.BadRequest => StatusCode.InvalidArgument,
            ResponseStatus.NotFound => StatusCode.NotFound,
            ResponseStatus.Conflict => StatusCode.AlreadyExists,
            ResponseStatus.Unauthorized => StatusCode.Unauthenticated,
            ResponseStatus.Forbidden => StatusCode.PermissionDenied,
            ResponseStatus.RequestTimeout or ResponseStatus.GatewayTimeout => StatusCode.DeadlineExceeded,
            ResponseStatus.TooManyRequests => StatusCode.ResourceExhausted,
            ResponseStatus.NotImplemented => StatusCode.Unimplemented,
            ResponseStatus.ServiceUnavailable => StatusCode.Unavailable,
            _ => StatusCode.Internal
        };
        var message = status == StatusCode.Internal ? "An internal error occurred." : error.Message;
        return new RpcException(new Status(status, message));
    }
}
