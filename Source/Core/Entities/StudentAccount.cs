namespace Core.Entities;

public class StudentAccount
{
    public Guid StudentAccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;
}
