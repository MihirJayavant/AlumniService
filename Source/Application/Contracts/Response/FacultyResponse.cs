namespace Application.Contracts.Response;

public class FacultyResponse
{
    public int FacultyId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public bool Admin { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public long MobileNo { get; set; }
    public string Extension { get; set; } = string.Empty;
}

