namespace AlumniBackendServices.Controllers;

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
            new FacultyController().Add(app);
            new FurtherStudiesController().Add(app);
            new StudentController().Add(app);
        }
    }
}