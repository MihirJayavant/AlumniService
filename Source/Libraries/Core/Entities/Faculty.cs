namespace Core.Entities;

public class Faculty
{
    public int FacultyId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long MobileNo { get; set; }
    public DateTime DateCreated { get; set; }
}
