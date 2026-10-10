namespace Alumni.Api.Controllers;

public sealed class ExamController : IEndpoint
{
    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/exam").WithTags(["Exam"]);
        api.MapGet("/{studentId:guid}", GetAsync).Produces<PaginatedList<ExamResponse>>();
        api.MapPost("/", PostAsync).Produces<ExamResponse>();
    }

    private static async Task<IResult> GetAsync(Guid studentId, GetExamHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetExam { StudentId = studentId };
        var result = await handler.Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddExam exam, AddExamHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.Execute(exam, cancellationToken);
        return response.ToServerResult();
    }
}
