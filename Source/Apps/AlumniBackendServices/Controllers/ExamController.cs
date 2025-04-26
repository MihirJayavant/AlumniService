using Application.Exams;

namespace AlumniBackendServices.Controllers;

public static class ExamController
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/exam");

        api.MapGet("/{studentId}", GetAsync).Produces<IEnumerable<ExamResponse>>();
        api.MapPost("/", PostAsync).Produces<ExamResponse>();
    }

    private static async Task<IResult> GetAsync(int studentId, IMediator mediator)
    {
        var query = new GetExamQuery(studentId);
        var result = await mediator.Send(query);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync([FromBody] AddExamCommand exam, IMediator mediator)
    {
        var response = await mediator.Send(exam);
        return response.ToServerResult();
    }
}
