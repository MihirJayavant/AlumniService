using HotChocolate;
using HotChocolate.Types;

namespace Alumni.Api.GraphQL;

[ExtendObjectType("Mutation")]
public sealed class FacultyMutation
{
    [GraphQLName("addFaculty")]
    public async Task<FacultyResponse?> AddFacultyAsync(
        AddFaculty input,
        [Service] IFacultyDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await new AddFacultyHandler(context).Execute(input, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }

    [GraphQLName("deleteFaculty")]
    public async Task<FacultyResponse?> DeleteFacultyAsync(
        Guid facultyId,
        [Service] IFacultyDbContext context,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFaculty { FacultyId = facultyId };
        var result = await new DeleteFacultyHandler(context).Execute(command, cancellationToken);
        return GraphQLResult.Unwrap(result);
    }
}
