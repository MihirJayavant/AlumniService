namespace Infrastructure.Commands;

public class AddCompanyCommand : IRequest<Response<CompanyResponse>>
{
    public string CompanyName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int YearOfJoining { get; set; }
    public long AnnualSalary { get; set; }
    public int StudentId { get; set; }
}
