namespace Alumni.Network;

[RecordView(typeof(Member))]
public partial record MemberResponse
{

}

public static class MemberResponseMapper
{
    public static MemberResponse ToMemberResponse(this MemberEntity member) =>
        new()
        {
            MemberId = member.MemberId,
            FirstName = member.FirstName,
            LastName = member.LastName,
            MobileNo = member.MobileNo,
            Extension = member.Extension,
            Gender = member.Gender,
            DateOfBirth = member.DateOfBirth,
            Email = member.Email,
            Branch = member.Branch,
            CurrentAddress = member.CurrentAddress,
            CorrespondenceAddress = member.CorrespondenceAddress,
            AdmissionYear = member.AdmissionYear,
            PassingYear = member.PassingYear,
        };
}
