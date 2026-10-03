using Xunit;

namespace Alumni.Faculty.UnitTests;

public sealed class FacultyResponseMapperTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToFacultyResponse_WhenFacultyHasAuditValues_PreservesAllExposedFields(bool hasUpdatedAt)
    {
        var createdAt = new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);
        var faculty = new Faculty
        {
            Id = 42,
            FacultyId = Guid.Parse("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1"),
            Email = "alice+faculty@example.com",
            FirstName = "Élodie",
            LastName = "D'Souza-Smith",
            Extension = "+91",
            MobileNo = 9876543210,
            CreatedAt = createdAt,
            UpdatedAt = hasUpdatedAt ? createdAt.AddDays(1) : null,
            IsDeleted = true
        };

        var response = faculty.ToFacultyResponse();

        Assert.Equal(faculty.FacultyId, response.FacultyId);
        Assert.Equal(faculty.Email, response.Email);
        Assert.Equal(faculty.FirstName, response.FirstName);
        Assert.Equal(faculty.LastName, response.LastName);
        Assert.Equal(faculty.Extension, response.Extension);
        Assert.Equal(faculty.MobileNo, response.MobileNo);
        Assert.Equal(faculty.CreatedAt, response.CreatedAt);
        Assert.Equal(faculty.UpdatedAt, response.UpdatedAt);
    }
}
