namespace Alumni.Auth;

/// <summary>Operation eligibility policies; resource ownership is enforced separately by use cases.</summary>
public static class AuthPermissions
{
    public const string StudentsRead = "Students.Read";
    public const string StudentsReadAll = "Students.ReadAll";
    public const string StudentsWrite = "Students.Write";

    // Eligibility only: ownership and the approved field list must also be checked by the use case.
    public const string StudentsSelfService = "Students.SelfService";

    public const string FacultyManage = "Faculty.Manage";
    public const string InvitationsManage = "Invitations.Manage";
}
