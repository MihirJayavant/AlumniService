namespace Alumni.Api.Controllers;

public interface IEndpoint
{
    public void Add(IEndpointRouteBuilder app);
}

public static class EndpointExtension
{
    extension(IEndpointRouteBuilder app)
    {
        public void AddControllers()
        {
            new CompanyController().Add(app);
            new ExamController().Add(app);
            new StaffController().Add(app);
            new FurtherStudiesController().Add(app);
            new MembersController().Add(app);
            new BootstrapController().Add(app);
        }
    }
}
