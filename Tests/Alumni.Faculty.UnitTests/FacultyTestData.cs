namespace Alumni.Faculty.UnitTests;

internal static class FacultyTestData
{
    public static AddFaculty ValidAddFaculty() => new()
    {
        Email = "alice@example.com",
        FirstName = "Alice",
        LastName = "D'Souza",
        Extension = "+91",
        MobileNo = 9876543210
    };
}
