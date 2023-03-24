namespace Application.Common.Commands;

public class AddStudentCommand : IRequest<Response<StudentResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public long MobileNo { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public string Branch { get; set; } = string.Empty;
    public AddressResponse CurrentAddress { get; set; } = null!;
    public AddressResponse CorrespondenceAddress { get; set; } = null!;
    public int AdmissionYear { get; set; }
    public int PassingYear { get; set; }

}
