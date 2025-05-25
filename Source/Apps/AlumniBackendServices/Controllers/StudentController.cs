namespace AlumniBackendServices.Controllers;

public static class StudentController
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/student");

        api.MapGet("/", GetAllAsync).Produces<PaginatedList<StudentResponse>>();
        api.MapGet("/{id:guid}", GetByEmail).Produces<StudentResponse>();
        api.MapPost("/", PostAsync).Produces<StudentResponse>();
    }

    private static async Task<IResult> GetAllAsync(int pageNumber, int pageSize, IStudentDbContext context, CancellationToken token)
    {
        var query = new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize };
        var response = await new GetAllStudentHandler(context).Execute(query, token);
        return response.ToServerResult();
    }

    private static async Task<IResult> GetByEmail(Guid id, IStudentDbContext context, CancellationToken token)
    {
        var query = new GetStudent { Id = id };
        var response = await new GetStudentHandler(context).Execute(query, token);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddStudent student, IStudentDbContext context, CancellationToken token)
    {
        var response = await new AddStudentHandler(context).Execute(student, token);
        return response.ToServerResult();
    }

}
