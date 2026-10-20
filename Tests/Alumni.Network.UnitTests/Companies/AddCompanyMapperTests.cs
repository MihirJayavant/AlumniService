using Alumni.Network.Companies;
using Xunit;

namespace Alumni.Network.UnitTests.Companies;

public sealed class AddCompanyMapperTests
{
    [Fact]
    public void ToCompany_WhenRequestHasPaddedText_TrimsAndPreservesContentAndInitializesIdentity()
    {
        var request = new AddCompany
        {
            MemberId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            CompanyName = "  École  Systems \t",
            Designation = "  Senior  Engineer \t",
            YearOfJoining = 2023,
            AnnualSalary = 1234567,
        };
        var member = MemberTestData.ValidAddMember().ToMember() with { Id = 42 };

        var entity = request.ToCompany(member);

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
        Assert.Equal(member.Id, entity.MemberId);
        Assert.Same(member, entity.Member);
    }
}
