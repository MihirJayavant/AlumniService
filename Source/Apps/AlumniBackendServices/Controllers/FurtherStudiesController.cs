namespace AlumniBackendServices.Controllers;

public static class FurtherStudiesController
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/further-studies").WithOpenApi().WithTags(["FurtherStudies"]);

        api.MapGet("/{studentId:guid}", GetAsync).Produces<IEnumerable<FurtherStudyResponse>>();
        api.MapPost("/", PostAsync).Produces<FurtherStudyResponse>();
    }

    private static async Task<IResult> GetAsync(Guid studentId, IStudentDbContext context, CancellationToken token)
    {
        var query = new GetFurtherStudy { StudentId = studentId };
        var result = await new GetFurtherStudyHandler(context).Execute(query, token);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddFurtherStudy study, IStudentDbContext context, CancellationToken token)
    {
        var result = await new AddFurtherStudyHandler(context).Execute(study, token);
        return result.ToServerResult();
    }

}
