using Grpc.Core;

namespace Alumni.Api.Grpc;

public sealed class ExamGrpc(
    GetExamHandler getExamHandler,
    AddExamHandler addExamHandler) : Contracts.ExamService.ExamServiceBase
{
    public override async Task<Contracts.ExamListReply> ListByStudent(Contracts.StudentIdRequest request, ServerCallContext context)
    {
        var query = new GetExam { StudentId = GrpcInput.Guid(request.StudentId, "student_id") };
        var result = await getExamHandler.Execute(query, context.CancellationToken);
        return GrpcResult.Map(result, page =>
        {
            var reply = new Contracts.ExamListReply { Pagination = GrpcMapping.Pagination(page) };
            reply.Items.AddRange(page.Items.Select(Map));
            return reply;
        });
    }

    public override async Task<Contracts.ExamReply> Add(Contracts.AddExamRequest request, ServerCallContext context)
    {
        var command = new AddExam
        {
            StudentId = GrpcInput.Guid(request.StudentId, "student_id"),
            ExamId = GrpcInput.OptionalGuid(request.ExamId, "exam_id"),
            ExamName = request.ExamName,
            Score = request.Score,
            Year = request.Year,
        };
        var result = await addExamHandler.Execute(command, context.CancellationToken);
        return GrpcResult.Map(result, Map);
    }

    private static Contracts.ExamReply Map(ExamResponse exam) => new()
    {
        ExamId = exam.ExamId.ToString(),
        ExamName = exam.ExamName,
        Score = exam.Score,
        Year = exam.Year,
    };
}
