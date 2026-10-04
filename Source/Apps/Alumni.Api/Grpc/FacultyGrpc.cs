using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class FacultyGrpc(IFacultyDbContext dbContext) : Contracts.FacultyService.FacultyServiceBase
{
    public override async Task<Contracts.FacultyListReply> List(Contracts.PaginationRequest request, ServerCallContext context)
    {
        var query = new GetAllFaculties
        {
            PageNumber = request.HasPageNumber ? request.PageNumber : 1,
            PageSize = request.HasPageSize ? request.PageSize : 10
        };
        var result = await new GetAllFacultiesHandler(dbContext).Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, page =>
        {
            var reply = new Contracts.FacultyListReply { Pagination = GrpcMapping.Pagination(page) };
            reply.Items.AddRange(page.Items.Select(ToReply));
            return reply;
        });
    }

    public override async Task<Contracts.FacultyReply> Get(Contracts.FacultyIdRequest request, ServerCallContext context)
    {
        var query = new GetFaculty { FacultyId = GrpcInput.Guid(request.FacultyId, "faculty_id") };
        var result = await new GetFacultyHandler(dbContext).Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    public override async Task<Contracts.FacultyReply> Add(Contracts.AddFacultyRequest request, ServerCallContext context)
    {
        var command = new AddFaculty
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Extension = request.Extension,
            MobileNo = request.MobileNo
        };
        var result = await new AddFacultyHandler(dbContext).Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    public override async Task<Contracts.FacultyReply> Delete(Contracts.FacultyIdRequest request, ServerCallContext context)
    {
        var command = new DeleteFaculty { FacultyId = GrpcInput.Guid(request.FacultyId, "faculty_id") };
        var result = await new DeleteFacultyHandler(dbContext).Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, ToReply);
    }

    private static Contracts.FacultyReply ToReply(FacultyResponse faculty)
    {
        var reply = new Contracts.FacultyReply
        {
            FacultyId = faculty.FacultyId.ToString("D"),
            Email = faculty.Email,
            FirstName = faculty.FirstName,
            LastName = faculty.LastName,
            Extension = faculty.Extension,
            MobileNo = faculty.MobileNo,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(faculty.CreatedAt, DateTimeKind.Utc))
        };
        if (faculty.UpdatedAt is { } updatedAt)
        {
            reply.UpdatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(updatedAt, DateTimeKind.Utc));
        }

        return reply;
    }
}
