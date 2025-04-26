using Application.Companies;

namespace AlumniBackendServices.Controllers;

public static class CompanyController
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/company").RequireAuthorization("StudentAccess");

        api.MapGet("/{studentId}", GetByIdAsync).Produces<IEnumerable<CompanyResponse>>();
        api.MapPost("/", PostAsync).Produces<CompanyResponse>();
    }

    private static async Task<IResult> GetByIdAsync(int studentId, IMediator mediator)
    {
        var query = new GetCompanyQuery(studentId);
        var response = await mediator.Send(query);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync([FromBody] AddCompanyCommand company, IMediator mediator)
    {
        var response = await mediator.Send(company);
        return response.ToServerResult();
    }

}
