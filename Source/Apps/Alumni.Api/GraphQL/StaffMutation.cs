using HotChocolate;
using HotChocolate.Types;

namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class StaffMutation
{
    [GraphQLName("addFaculty")]
    public async Task<StaffMemberResponse?> AddFacultyAsync(
        AddStaffMember input,
        [Service] AddStaffMemberHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("deleteFaculty")]
    public async Task<StaffMemberResponse?> DeleteFacultyAsync(
        Guid facultyId,
        [Service] DeleteStaffMemberHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteStaffMember { FacultyId = facultyId };
        var result = await handler.Execute(command, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
