namespace AlumniBackendServices.Controllers;

public static class ExamController
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/exam").WithOpenApi().WithTags(["Exam"]);
        api.MapGet("/{studentId:guid}", GetAsync).Produces<PaginatedList<ExamResponse>>();
        api.MapPost("/", PostAsync).Produces<ExamResponse>();
    }

    private static async Task<IResult> GetAsync(Guid studentId, IStudentDbContext context, CancellationToken cancellationToken)
    {
        var query = new GetExam { StudentId = studentId };
        var result = await new GetExamHandler(context).Execute(query, cancellationToken);
        return result.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddExam exam, IStudentDbContext context, CancellationToken cancellationToken)
    {
        var response = await new AddExamHandler(context).Execute(exam, cancellationToken);
        return response.ToServerResult();
    }
}
