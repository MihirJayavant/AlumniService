using Grpc.Core;

namespace AlumniBackendServices.Grpc;

public class IdentityGrpc : Identity.IdentityBase
{
    private readonly IMediator mediator;
    public IdentityGrpc(IMediator mediator) => this.mediator = mediator;

    public override Task<IdentityResponse> StudentRegister(IdentityRequest request, ServerCallContext context)
    {
        return Task.FromResult(new IdentityResponse() { Message = "Done" });
    }
}
