namespace Alumni.Api.Controllers;

public sealed class CompanyController : IEndpoint
{

    public void Add(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/company").WithTags(["Company"]);

        api.MapGet("/{studentId:guid}", GetByIdAsync).Produces<PaginatedList<CompanyResponse>>();
        api.MapPost("/", PostAsync).Produces<CompanyResponse>();
    }

    private static async Task<IResult> GetByIdAsync(Guid studentId, GetCompanyHandler handler, CancellationToken cancellationToken)
    {
        var query = new GetCompany { StudentId = studentId };
        var response = await handler.Execute(query, cancellationToken);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddCompany company, AddCompanyHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.Execute(company, cancellationToken);
        return response.ToServerResult();
    }

}
