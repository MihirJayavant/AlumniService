namespace Alumni.Api.Controllers;

using Microsoft.AspNetCore.OpenApi;

public sealed class StudentController : IEndpoint
{
    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/student");

        api.MapGet("/", GetAllAsync).Produces<PaginatedList<StudentResponse>>();
        api.MapGet("/{id:guid}", GetByEmail).Produces<StudentResponse>();
        api.MapPost("/", PostAsync).Produces<StudentResponse>();
    }

    private static async Task<IResult> GetAllAsync(int pageNumber, int pageSize, GetAllStudentHandler handler, CancellationToken token)
    {
        var query = new GetAllStudent { PageNumber = pageNumber, PageSize = pageSize };
        var response = await handler.Execute(query, token);
        return response.ToServerResult();
    }

    private static async Task<IResult> GetByEmail(Guid id, GetStudentHandler handler, CancellationToken token)
    {
        var query = new GetStudent { Id = id };
        var response = await handler.Execute(query, token);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddStudent student, AddStudentHandler handler, CancellationToken token)
    {
        var response = await handler.Execute(student, token);
        return response.ToServerResult();
    }

}
