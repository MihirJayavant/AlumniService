namespace Infrastructure.Queries;

public class GetStudentQuery : IRequest<Response<StudentResponse>>
{
    public string Email { get; }

    public GetStudentQuery(string email) => Email = email;
}
