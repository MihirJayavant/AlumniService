namespace Application.Companies;

public sealed record CompanyResponse
{
    public required string CompanyName { get; init; }
    public required string Designation { get; init; }
    public required int YearOfJoining { get; init; }
    public required long AnnualSalary { get; init; }
}
