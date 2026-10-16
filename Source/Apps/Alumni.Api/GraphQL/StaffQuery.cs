namespace Alumni.Api.GraphQL;

[ExtendObjectType("Query")]
public sealed class StaffQuery
{
    [GraphQLName("faculties")]
    public async Task<PaginatedList<StaffMemberResponse>?> GetFacultiesAsync(
        int pageNumber,
        int pageSize,
        [Service] GetStaffMembersHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetStaffMembers { PageNumber = pageNumber, PageSize = pageSize };
        var result = await handler.Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("faculty")]
    public async Task<StaffMemberResponse?> GetFacultyAsync(
        Guid facultyId,
        [Service] GetStaffMemberHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetStaffMember { FacultyId = facultyId };
        var result = await handler.Execute(query, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
