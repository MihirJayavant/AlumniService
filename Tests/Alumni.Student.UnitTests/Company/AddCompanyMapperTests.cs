using Alumni.Student.Company;
using Xunit;

namespace Alumni.Student.UnitTests.Company;

public sealed class AddCompanyMapperTests
{
    [Fact]
    public void ToCompany_WhenRequestHasPaddedText_TrimsAndPreservesContentAndInitializesIdentity()
    {
        var request = new AddCompany
        {
            StudentId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            CompanyName = "  École  Systems \t",
            Designation = "  Senior  Engineer \t",
            YearOfJoining = 2023,
            AnnualSalary = 1234567,
        };
        var student = StudentTestData.ValidAddStudent().ToStudent() with { Id = 42 };

        var entity = request.ToCompany(student);

        Assert.Equal("École  Systems", entity.CompanyName);
        Assert.Equal("  École  Systems \t", request.CompanyName);
        Assert.Equal("Senior  Engineer", entity.Designation);
        Assert.Equal("  Senior  Engineer \t", request.Designation);
        Assert.Equal(request.YearOfJoining, entity.YearOfJoining);
        Assert.Equal(request.AnnualSalary, entity.AnnualSalary);
        Assert.Equal(0, entity.Id);
        Assert.NotEqual(Guid.Empty, entity.CompanyId);
        Assert.NotEqual(request.CompanyId, entity.CompanyId);
        Assert.Equal(7, entity.CompanyId.Version);
        Assert.Equal(student.Id, entity.StudentId);
        Assert.Same(student, entity.Student);
    }
}
