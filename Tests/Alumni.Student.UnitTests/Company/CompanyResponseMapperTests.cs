using Alumni.Student.Company;
using Xunit;
using CompanyModel = Alumni.Student.Company.Company;

namespace Alumni.Student.UnitTests.Company;

public sealed class CompanyResponseMapperTests
{
    [Fact]
    public void ToCompanyResponse_WhenCompanyHasValues_PreservesAllExposedFields()
    {
        var company = new CompanyModel
        {
            Id = 42,
            CompanyId = Guid.Parse("cf3a4b49-7b78-4bd2-b51c-5ea3a428f99a"),
            CompanyName = "Alumni Systems",
            Designation = "Software Engineer",
            YearOfJoining = 2023,
            AnnualSalary = 1234567
        };

        var response = company.ToCompanyResponse();

        Assert.Equal(company.CompanyId, response.CompanyId);
        Assert.Equal(company.CompanyName, response.CompanyName);
        Assert.Equal(company.Designation, response.Designation);
        Assert.Equal(company.YearOfJoining, response.YearOfJoining);
        Assert.Equal(company.AnnualSalary, response.AnnualSalary);
    }
}
