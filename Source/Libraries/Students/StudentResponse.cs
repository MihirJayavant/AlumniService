using Domain;

namespace Students;

[RecordView(typeof(Student), nameof(Student.Uuid), nameof(Student.Id))]
public partial record StudentResponse
{
    public required Guid Id { get; set; }
}

public static class StudentResponseMapper
{
    public static StudentResponse ToStudentResponse(this Student student) =>
        new()
        {
            Id = student.Uuid,
            FirstName = student.FirstName,
            LastName = student.LastName,
            MobileNo = student.MobileNo,
            Extension = student.Extension,
            Gender = student.Gender,
            DateOfBirth = student.DateOfBirth,
            Email = student.Email,
            Branch = student.Branch,
            CurrentAddress = student.CurrentAddress,
            CorrespondenceAddress = student.CorrespondenceAddress,
            AdmissionYear = student.AdmissionYear,
            PassingYear = student.PassingYear
        };
}
