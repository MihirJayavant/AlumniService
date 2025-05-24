namespace Alumni.Student.Companies;

public class Company : IEntity
{
    public required int Id { get; init; }
    public required Guid CompanyId { get; init; }
    public required string CompanyName { get; init; }
    public required string Designation { get; init; }
    public required int YearOfJoining { get; init; }
    public required long AnnualSalary { get; init; }
    public required int StudentId { get; init; }
    public required Student Student { get; init; }
}
