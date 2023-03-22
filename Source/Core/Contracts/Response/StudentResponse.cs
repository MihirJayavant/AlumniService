namespace Core.Contracts.Response;

public class StudentResponse
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public long MobileNo { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public string Branch { get; set; } = string.Empty;
    public AddressResponse CurrentAddress { get; set; } = default!;
    public AddressResponse CorrespondanceAddress { get; set; } = default!;
    public int AdmissionYear { get; set; }
    public int PassingYear { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateLastModified { get; set; }

}
