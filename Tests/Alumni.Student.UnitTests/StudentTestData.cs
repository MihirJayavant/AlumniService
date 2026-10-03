namespace Alumni.Student.UnitTests;

internal static class StudentTestData
{
    public static AddStudent ValidAddStudent() => new()
    {
        StudentId = Guid.Parse("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1"),
        FirstName = "Élodie",
        LastName = "D'Souza-Smith",
        MobileNo = "9876543210",
        Extension = "+91",
        Gender = Gender.Female,
        DateOfBirth = new DateOnly(1998, 4, 12),
        Email = "alice+alumni@example.com",
        Branch = Branch.IT,
        CurrentAddress = new Address
        {
            PinCode = "400001",
            Country = "India",
            State = "Maharashtra",
            City = "Mumbai",
            UserAddress = "12 Current Street"
        },
        CorrespondenceAddress = new Address
        {
            PinCode = "560001",
            Country = "India",
            State = "Karnataka",
            City = "Bengaluru",
            UserAddress = "34 Correspondence Road"
        },
        AdmissionYear = 2016,
        PassingYear = 2020
    };
}
