namespace Alumni.Api.Controllers;

public sealed class StaffController : IEndpoint
{

    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/faculty").WithTags(["Faculty"]);

        api.MapGet("/", GetAsync).Produces<PaginatedList<StaffMemberResponse>>();
        api.MapGet("/{facultyId:guid}", GetByEmailAsync).Produces<StaffMemberResponse>();
        api.MapPost("/", PostAsync);
        api.MapDelete("/{facultyId:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetAsync(int pageNumber, int pageSize, GetStaffMembersHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetStaffMembers { PageNumber = pageNumber, PageSize = pageSize };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> GetByEmailAsync(Guid facultyId, GetStaffMemberHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetStaffMember { FacultyId = facultyId };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddStaffMember faculty, AddStaffMemberHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.Execute(faculty, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> DeleteAsync(Guid facultyId, DeleteStaffMemberHandler handler, CancellationToken cancellationToken)
    {
        var query = new DeleteStaffMember { FacultyId = facultyId };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }
}
