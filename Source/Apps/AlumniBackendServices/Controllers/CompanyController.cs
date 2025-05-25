namespace AlumniBackendServices.Controllers;

public static class CompanyController
{

    public static void Add(WebApplication app)
    {
        var api = app.MapGroup("/company").WithOpenApi().WithTags([ "Company"]);

        api.MapGet("/{studentId:guid}", GetByIdAsync).Produces<PaginatedList<CompanyResponse>>();
        api.MapPost("/", PostAsync).Produces<CompanyResponse>();
    }

    private static async Task<IResult> GetByIdAsync(Guid studentId, IStudentDbContext context, CancellationToken cancellationToken)
    {
        var query = new GetCompany { StudentId = studentId };
        var response = await  new GetCompanyHandler(context).Execute(query, cancellationToken);
        return response.ToServerResult();
    }

    private static async Task<IResult> PostAsync(AddCompany company, IStudentDbContext context, CancellationToken cancellationToken)
    {
        var response = await new AddCompanyHandler(context).Execute(company, cancellationToken);
        return response.ToServerResult();
    }

}
