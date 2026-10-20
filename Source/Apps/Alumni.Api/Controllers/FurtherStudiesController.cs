namespace Alumni.Api.Controllers;

public sealed class FurtherStudiesController : IEndpoint
{

    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/further-studies").WithTags(["FurtherStudies"]);

        api.MapGet("/{memberId:guid}", GetAsync).Produces<PaginatedList<FurtherStudyResponse>>();
        api.MapPost("/", PostAsync).Produces<FurtherStudyResponse>();
    }

    private static async Task<IResult> GetAsync(Guid memberId, GetFurtherStudyHandler handler, CancellationToken token)
    {
        var query = new GetFurtherStudy { MemberId = memberId };
        var result = await handler.Execute(query, token);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddFurtherStudy study, AddFurtherStudyHandler handler, CancellationToken token)
    {
        var result = await handler.Execute(study, token);
        return result.ToServerResult();
    }

}
