using Microsoft.AspNetCore.Mvc;

namespace AlumniBackendServices.Controllers;

public class FurtherStudiesController : IEndpoint
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/further-studies");

        api.MapGet("/{studentId}", GetAsync).Produces<IEnumerable<FurtherStudyResponse>>();
        api.MapPost("/", PostAsync).Produces<FurtherStudyResponse>();
    }

    public static async Task<IResult> GetAsync(int studentId, IMediator mediator)
    {
        var query = new GetFurtherStudyQuery(studentId);
        var result = await mediator.Send(query);
        return EndpointHelper.GetResult(result);
    }

    public static async Task<IResult> PostAsync(AddFurtherStudyCommand study, IMediator mediator)
    {
        var result = await mediator.Send(study);
        return EndpointHelper.GetResult(result);
    }

}
