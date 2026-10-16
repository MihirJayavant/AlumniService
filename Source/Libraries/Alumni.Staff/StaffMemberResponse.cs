namespace Alumni.Staff;

[RecordView(typeof(StaffMember))]
public sealed partial record StaffMemberResponse
{
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}

public static class StaffMemberResponseMapper
{
    public static StaffMemberResponse ToStaffMemberResponse(this StaffMemberEntity staffMember) =>
        new()
        {
            FacultyId = staffMember.FacultyId,
            FirstName = staffMember.FirstName,
            LastName = staffMember.LastName,
            MobileNo = staffMember.MobileNo,
            Extension = staffMember.Extension,
            Email = staffMember.Email,
            CreatedAt = staffMember.CreatedAt,
            UpdatedAt = staffMember.UpdatedAt,
        };
}
