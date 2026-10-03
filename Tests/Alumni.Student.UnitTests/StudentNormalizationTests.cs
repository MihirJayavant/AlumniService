using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class StudentNormalizationTests
{
    [Theory]
    [InlineData(" male ", " it ", "Male", "it")]
    [InlineData(" FEMALE\t", "cS", "Female", "cS")]
    [InlineData("mAlE", " exTc ", "Male", "exTc")]
    [InlineData(" female ", "eLeX", "Female", "eLeX")]
    public void ToStudent_WhenChoicesHaveDifferentCasing_CanonicalizesGenderAndTrimsBranchWithoutMutatingInput(
        string gender, string branch, string expectedGender, string expectedBranch)
    {
        var request = StudentTestData.ValidAddStudent() with { Gender = gender, Branch = branch };
        var validation = new AddStudentHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        var student = request.ToStudent();

        Assert.True(validation.IsValid);
        Assert.Equal(expectedGender, student.Gender);
        Assert.Equal(expectedBranch, student.Branch);
        Assert.Equal(gender, request.Gender);
        Assert.Equal(branch, request.Branch);
    }

    [Fact]
    public void ToStudent_WhenTextIsPadded_TrimsAllFieldsAndPreservesInternalTextWithoutMutatingInput()
    {
        var request = StudentTestData.ValidAddStudent() with
        {
            FirstName = " \tÉlodie  Anne \r\n",
            LastName = " D'Souza-Smith ",
            MobileNo = " \t009876543210 \n",
            Extension = " +91 ",
            Email = " Alice+Alumni@Example.COM ",
            CurrentAddress = new Address
            {
                PinCode = " 400 001 ",
                Country = " Côte d'Ivoire ",
                State = " Maharashtra ",
                City = " Navi  Mumbai ",
                UserAddress = " 12, Rue de l'Église – Apt.  4 "
            },
            CorrespondenceAddress = new Address
            {
                PinCode = " SW1A 1AA ",
                Country = " United Kingdom ",
                State = " Greater London ",
                City = " London ",
                UserAddress = " 34 King's Road, Flat  2 "
            }
        };
        var original = request with { };
        var currentAddress = request.CurrentAddress;
        var correspondenceAddress = request.CorrespondenceAddress;
        var validation = new AddStudentHandler(new RejectingStudentDbContext()).Validator.Validate(request);

        var student = request.ToStudent();

        Assert.True(validation.IsValid);
        Assert.Equal("Élodie  Anne", student.FirstName);
        Assert.Equal("D'Souza-Smith", student.LastName);
        Assert.Equal("009876543210", student.MobileNo);
        Assert.Equal("+91", student.Extension);
        Assert.Equal("alice+alumni@example.com", student.Email);
        Assert.Equal(new Address
        {
            PinCode = "400 001",
            Country = "Côte d'Ivoire",
            State = "Maharashtra",
            City = "Navi  Mumbai",
            UserAddress = "12, Rue de l'Église – Apt.  4"
        }, student.CurrentAddress);
        Assert.Equal(new Address
        {
            PinCode = "SW1A 1AA",
            Country = "United Kingdom",
            State = "Greater London",
            City = "London",
            UserAddress = "34 King's Road, Flat  2"
        }, student.CorrespondenceAddress);
        Assert.Equal(original, request);
        Assert.Same(currentAddress, request.CurrentAddress);
        Assert.Same(correspondenceAddress, request.CorrespondenceAddress);
        Assert.Equal(" 400 001 ", request.CurrentAddress.PinCode);
        Assert.Equal(" 34 King's Road, Flat  2 ", request.CorrespondenceAddress.UserAddress);
    }
}
