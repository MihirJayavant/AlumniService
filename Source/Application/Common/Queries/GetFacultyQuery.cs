namespace Infrastructure.Queries;

public class GetFacultyQuery : IRequest<FacultyResponse>
{
    public string FacultyEmail { get; }

    public GetFacultyQuery(string facultyEmail) =>FacultyEmail = facultyEmail;
}
