namespace Core.Entities;

public class StudentAccount
{
    public Guid StudentAccountId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; }
}
