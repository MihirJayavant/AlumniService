namespace Application.Contracts.Response;

public class CompanyResponse
{
    public string CompanyName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int YearOfJoining { get; set; }
    public long AnnualSalary { get; set; }
}
