using HotChocolate;
using HotChocolate.Types;

namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class FacultyMutation
{
    [GraphQLName("addFaculty")]
    public async Task<FacultyResponse?> AddFacultyAsync(
        AddFaculty input,
        [Service] AddFacultyHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("deleteFaculty")]
    public async Task<FacultyResponse?> DeleteFacultyAsync(
        Guid facultyId,
        [Service] DeleteFacultyHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFaculty { FacultyId = facultyId };
        var result = await handler.Execute(command, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
