using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class IdentityGrpc : Identity.IdentityBase
{
    public override Task<IdentityResponse> StudentRegister(IdentityRequest request, ServerCallContext context)
        => Task.FromResult(new IdentityResponse() { Message = "Done" });
}
