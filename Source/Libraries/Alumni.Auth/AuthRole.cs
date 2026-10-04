using Microsoft.AspNetCore.Identity;

namespace Alumni.Auth;

public class AuthRole : IdentityRole
{
}

public static class AuthRoles
{
    public const string Student = "Student";
    public const string FacultyReader = "FacultyReader";
    public const string FacultyEditor = "FacultyEditor";
    public const string FacultyAdmin = "FacultyAdmin";
}
