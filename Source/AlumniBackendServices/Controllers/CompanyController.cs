using Application.Companies;
using Microsoft.AspNetCore.Mvc;

namespace AlumniBackendServices.Controllers;

public class CompanyController : IEndpoint
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/company");

        api.MapGet("/{studentId}", GetAsyncByID).Produces<IEnumerable<CompanyResponse>>();
        api.MapPost("/", PostAsync).Produces<CompanyResponse>();
    }

    public static async Task<IResult> GetAsyncByID(int studentId, IMediator mediator)
    {
        var query = new GetCompanyQuery(studentId);
        var response = await mediator.Send(query);
        return EndpointHelper.GetResult(response);
    }

    public static async Task<IResult> PostAsync([FromBody] AddCompanyCommand company, IMediator mediator)
    {
        var response = await mediator.Send(company);
        return EndpointHelper.GetResult(response);
    }

}
