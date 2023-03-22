namespace Core.Entities;

public class Company
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int YearOfJoining { get; set; }
    public long AnnualSalary { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

}
