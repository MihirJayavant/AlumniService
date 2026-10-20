namespace Alumni.Auth;

/// <summary>Operation eligibility policies; resource ownership is enforced separately by use cases.</summary>
public static class AuthPermissions
{
    public const string MembersRead = "Members.Read";
    public const string MembersReadAll = "Members.ReadAll";
    public const string MembersWrite = "Members.Write";

    // Eligibility only: ownership and the approved field list must also be checked by the use case.
    public const string MembersSelfService = "Members.SelfService";

    public const string FacultyManage = "Faculty.Manage";
    public const string InvitationsManage = "Invitations.Manage";
}
