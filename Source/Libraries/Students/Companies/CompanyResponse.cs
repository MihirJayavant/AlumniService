namespace Students.Companies;

[RecordView(typeof(Company), nameof(Company.Id), nameof(Company.Student), nameof(Company.StudentId))]
public sealed partial record CompanyResponse
{

}

public static class CompanyResponseMapper
{
    public static CompanyResponse ToCompanyResponse(this Company company) =>
        new()
        {

        };
}
