using Xunit;

namespace Alumni.Network.UnitTests;

public sealed class AddMemberMapperTests
{
    [Theory]
    [InlineData("Alice+Alumni@Example.COM", "alice+alumni@example.com")]
    [InlineData("  Alice+Alumni@Example.COM \t", "alice+alumni@example.com")]
    public void ToMember_WhenRequestIsValid_PreservesFieldsAndInitializesIdentityAndAudit(string email, string expectedEmail)
    {
        var request = MemberTestData.ValidAddMember() with { Email = email };
        var before = DateTime.UtcNow;

        var member = request.ToMember();

        var after = DateTime.UtcNow;
        Assert.Equal(0, member.Id);
        Assert.NotEqual(Guid.Empty, member.MemberId);
        Assert.NotEqual(request.MemberId, member.MemberId);
        Assert.Equal(7, member.MemberId.Version);
        Assert.Equal(expectedEmail, member.Email);
        Assert.Equal(email, request.Email);
        Assert.Equal(request.FirstName, member.FirstName);
        Assert.Equal(request.LastName, member.LastName);
        Assert.Equal(request.MobileNo, member.MobileNo);
        Assert.Equal(request.Extension, member.Extension);
        Assert.Equal(request.Gender, member.Gender);
        Assert.Equal(request.DateOfBirth, member.DateOfBirth);
        Assert.Equal(request.Branch, member.Branch);
        Assert.Equal(request.CurrentAddress, member.CurrentAddress);
        Assert.Equal(request.CorrespondenceAddress, member.CorrespondenceAddress);
        Assert.Equal(request.AdmissionYear, member.AdmissionYear);
        Assert.Equal(request.PassingYear, member.PassingYear);
        Assert.Equal(DateTimeKind.Utc, member.CreatedAt.Kind);
        Assert.InRange(member.CreatedAt, before, after);
        Assert.True(member.UpdatedAt.HasValue);
        Assert.Equal(DateTimeKind.Utc, member.UpdatedAt.Value.Kind);
        Assert.InRange(member.UpdatedAt.Value, before, after);
    }
}
