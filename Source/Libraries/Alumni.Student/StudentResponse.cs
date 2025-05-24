namespace Alumni.Student;

[RecordView(typeof(Student), nameof(Student.Id))]
public partial record StudentResponse
{

}

public static class StudentResponseMapper
{
    public static StudentResponse ToStudentResponse(this Student student) =>
        new()
        {
            StudentId = student.StudentId,
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
