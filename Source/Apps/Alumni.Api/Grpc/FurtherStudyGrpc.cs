using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class FurtherStudyGrpc(IStudentDbContext database) : Contracts.FurtherStudyService.FurtherStudyServiceBase
{
    public override async Task<Contracts.FurtherStudyListReply> ListByStudent(Contracts.StudentIdRequest request, ServerCallContext context)
    {
        var query = new GetFurtherStudy { StudentId = GrpcInput.Guid(request.StudentId, "student_id") };
        var result = await new GetFurtherStudyHandler(database).Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, page =>
        {
            var reply = new Contracts.FurtherStudyListReply { Pagination = GrpcMapping.Pagination(page) };
            reply.Items.AddRange(page.Items.Select(Map));
            return reply;
        });
    }

    public override async Task<Contracts.FurtherStudyReply> Add(Contracts.AddFurtherStudyRequest request, ServerCallContext context)
    {
        var command = new AddFurtherStudy
        {
            StudentId = GrpcInput.Guid(request.StudentId, "student_id"),
            FurtherStudyId = GrpcInput.OptionalGuid(request.FurtherStudyId, "further_study_id"),
            InstituteName = request.InstituteName,
            Degree = request.Degree,
            AdmissionYear = request.AdmissionYear,
            PassingYear = request.PassingYear,
            Country = request.Country,
            City = request.City,
        };
        var result = await new AddFurtherStudyHandler(database).Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, Map);
    }

    private static Contracts.FurtherStudyReply Map(FurtherStudyResponse study) => new()
    {
        FurtherStudyId = study.FurtherStudyId.ToString(),
        InstituteName = study.InstituteName,
        Degree = study.Degree,
        AdmissionYear = study.AdmissionYear,
        PassingYear = study.PassingYear,
        Country = study.Country,
        City = study.City,
    };
}
