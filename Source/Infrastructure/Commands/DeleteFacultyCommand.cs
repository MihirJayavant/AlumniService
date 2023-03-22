namespace Infrastructure.Commands;

public class DeleteFacultyCommand : IRequest
{
    public int FacultyId { get; set; }
    public DeleteFacultyCommand(int facultyId) => FacultyId = facultyId;
}
