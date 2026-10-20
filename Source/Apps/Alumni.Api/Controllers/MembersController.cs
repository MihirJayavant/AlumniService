namespace Alumni.Api.Controllers;

using Microsoft.AspNetCore.OpenApi;

public sealed class MembersController : IEndpoint
{
    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/members");

        api.MapGet("/", GetAllAsync).Produces<PaginatedList<MemberResponse>>();
        api.MapGet("/{id:guid}", GetByEmail).Produces<MemberResponse>();
        api.MapPost("/", PostAsync).Produces<MemberResponse>();
    }

    private static async Task<IResult> GetAllAsync(int pageNumber, int pageSize, GetMembersHandler handler, CancellationToken token)
    {
        var query = new GetMembers { PageNumber = pageNumber, PageSize = pageSize };
        var response = await handler.Execute(query, token);
        return response.ToServerResult();
    }

    private static async Task<IResult> GetByEmail(Guid id, GetMemberHandler handler, CancellationToken token)
    {
        var query = new GetMember { Id = id };
        var response = await handler.Execute(query, token);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddMember member, AddMemberHandler handler, CancellationToken token)
    {
        var response = await handler.Execute(member, token);
        return response.ToServerResult();
    }

}
