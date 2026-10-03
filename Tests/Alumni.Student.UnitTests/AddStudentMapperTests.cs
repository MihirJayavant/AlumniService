using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class AddStudentMapperTests
{
    [Theory]
    [InlineData("Alice+Alumni@Example.COM", "alice+alumni@example.com")]
    [InlineData("  Alice+Alumni@Example.COM \t", "alice+alumni@example.com")]
    public void ToStudent_WhenRequestIsValid_PreservesFieldsAndInitializesIdentityAndAudit(string email, string expectedEmail)
    {
        var request = StudentTestData.ValidAddStudent() with { Email = email };
        var before = DateTime.UtcNow;

        var student = request.ToStudent();

        var after = DateTime.UtcNow;
        Assert.Equal(0, student.Id);
        Assert.NotEqual(Guid.Empty, student.StudentId);
        Assert.NotEqual(request.StudentId, student.StudentId);
        Assert.Equal(7, student.StudentId.Version);
        Assert.Equal(expectedEmail, student.Email);
        Assert.Equal(email, request.Email);
        Assert.Equal(request.FirstName, student.FirstName);
        Assert.Equal(request.LastName, student.LastName);
        Assert.Equal(request.MobileNo, student.MobileNo);
        Assert.Equal(request.Extension, student.Extension);
        Assert.Equal(request.Gender, student.Gender);
        Assert.Equal(request.DateOfBirth, student.DateOfBirth);
        Assert.Equal(request.Branch, student.Branch);
        Assert.Equal(request.CurrentAddress, student.CurrentAddress);
        Assert.Equal(request.CorrespondenceAddress, student.CorrespondenceAddress);
        Assert.Equal(request.AdmissionYear, student.AdmissionYear);
        Assert.Equal(request.PassingYear, student.PassingYear);
        Assert.Equal(DateTimeKind.Utc, student.CreatedAt.Kind);
        Assert.InRange(student.CreatedAt, before, after);
        Assert.True(student.UpdatedAt.HasValue);
        Assert.Equal(DateTimeKind.Utc, student.UpdatedAt.Value.Kind);
        Assert.InRange(student.UpdatedAt.Value, before, after);
    }
}
