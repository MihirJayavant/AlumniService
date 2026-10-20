using Xunit;

namespace Alumni.Network.UnitTests;

public sealed class MemberResponseMapperTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToMemberResponse_WhenEntityHasDistinctFields_PreservesAllPublicFields(bool hasUpdatedAt)
    {
        var request = MemberTestData.ValidAddMember();
        var createdAt = new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);
        var member = new MemberEntity
        {
            MemberId = request.MemberId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MobileNo = request.MobileNo,
            Extension = request.Extension,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Branch = request.Branch,
            CurrentAddress = request.CurrentAddress,
            CorrespondenceAddress = request.CorrespondenceAddress,
            AdmissionYear = request.AdmissionYear,
            PassingYear = request.PassingYear,
            Id = 42,
            CreatedAt = createdAt,
            UpdatedAt = hasUpdatedAt ? createdAt.AddDays(1) : null,
            IsDeleted = true
        };

        var response = member.ToMemberResponse();

        Assert.Equal(member.MemberId, response.MemberId);
        Assert.Equal(member.Email, response.Email);
        Assert.Equal(member.FirstName, response.FirstName);
        Assert.Equal(member.LastName, response.LastName);
        Assert.Equal(member.MobileNo, response.MobileNo);
        Assert.Equal(member.Extension, response.Extension);
        Assert.Equal(member.Gender, response.Gender);
        Assert.Equal(member.DateOfBirth, response.DateOfBirth);
        Assert.Equal(member.Branch, response.Branch);
        Assert.Equal(member.CurrentAddress, response.CurrentAddress);
        Assert.Equal(member.CorrespondenceAddress, response.CorrespondenceAddress);
        Assert.Equal(member.AdmissionYear, response.AdmissionYear);
        Assert.Equal(member.PassingYear, response.PassingYear);
    }
}
