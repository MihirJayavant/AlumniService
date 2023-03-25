using Application.Students;
using Application.Students.Queries;
using Core.Contracts.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlumniBackendServices.Controllers;

public class StudentController : IEndpoint
{
    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/student");

        api.MapGet("/", GetAllAsync).Produces<AllStudentResponse>();
        api.MapGet("/{email}", GetByEmail).Produces<StudentResponse>();
        api.MapPost("/", PostAsync).Produces<FurtherStudyResponse>();
    }

    public static async Task<IResult> GetAllAsync(GetAllStudentRequest request, IMediator mediator)
    {
        var query = new GetAllStudentQuery(request.PageNumber, request.PageSize);
        var response = await mediator.Send(query);
        return EndpointHelper.GetResult(response);
    }

    public static async Task<IResult> GetByEmail(string email, IMediator mediator)
    {
        var query = new GetStudentQuery(email);
        var response = await mediator.Send(query);
        return EndpointHelper.GetResult(response);
    }

    public static async Task<IResult> PostAsync(AddStudentCommand student, IMediator mediator)
    {
        var response = await mediator.Send(student);
        return EndpointHelper.GetResult(response);
    }

}
