using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class StudentResponseMapperTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToStudentResponse_WhenEntityHasDistinctFields_PreservesAllPublicFields(bool hasUpdatedAt)
    {
        var request = StudentTestData.ValidAddStudent();
        var createdAt = new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);
        var student = new StudentEntity
        {
            StudentId = request.StudentId,
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

        var response = student.ToStudentResponse();

        Assert.Equal(student.StudentId, response.StudentId);
        Assert.Equal(student.Email, response.Email);
        Assert.Equal(student.FirstName, response.FirstName);
        Assert.Equal(student.LastName, response.LastName);
        Assert.Equal(student.MobileNo, response.MobileNo);
        Assert.Equal(student.Extension, response.Extension);
        Assert.Equal(student.Gender, response.Gender);
        Assert.Equal(student.DateOfBirth, response.DateOfBirth);
        Assert.Equal(student.Branch, response.Branch);
        Assert.Equal(student.CurrentAddress, response.CurrentAddress);
        Assert.Equal(student.CorrespondenceAddress, response.CorrespondenceAddress);
        Assert.Equal(student.AdmissionYear, response.AdmissionYear);
        Assert.Equal(student.PassingYear, response.PassingYear);
    }
}
