using Application.FurtherStudies;

namespace AlumniBackendServices.Controllers;

public static class FurtherStudiesController
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/further-studies");

        api.MapGet("/{studentId}", GetAsync).Produces<IEnumerable<FurtherStudyResponse>>();
        api.MapPost("/", PostAsync).Produces<FurtherStudyResponse>();
    }

    private static async Task<IResult> GetAsync(int studentId, IMediator mediator)
    {
        var query = new GetFurtherStudyQuery(studentId);
        var result = await mediator.Send(query);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync([FromBody] AddFurtherStudyCommand study, IMediator mediator)
    {
        var result = await mediator.Send(study);
        return result.ToServerResult();
    }

}
