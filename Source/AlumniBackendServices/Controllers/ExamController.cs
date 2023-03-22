using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlumniBackendServices.Controllers;

public sealed class ExamController : IEndpoint
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/exam");

        api.MapGet("/{studentId}", GetAsync).Produces<IEnumerable<ExamResponse>>();
        api.MapPost("/", PostAsync).Produces<ExamResponse>();
    }

    public static async Task<IResult> GetAsync(int studentId, IMediator mediator)
    {
        var query = new GetExamQuery(studentId);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> PostAsync(AddExamCommand exam, IMediator mediator)
    {
        var response = await mediator.Send(exam);
        return EndpointHelper.GetResult(response);
    }
}
