using Xunit;

namespace Alumni.Network.UnitTests;

public sealed class MemberNormalizationTests
{
    [Theory]
    [InlineData(" male ", " it ", "Male", "it")]
    [InlineData(" FEMALE\t", "cS", "Female", "cS")]
    [InlineData("mAlE", " exTc ", "Male", "exTc")]
    [InlineData(" female ", "eLeX", "Female", "eLeX")]
    public void ToMember_WhenChoicesHaveDifferentCasing_CanonicalizesGenderAndTrimsBranchWithoutMutatingInput(
        string gender, string branch, string expectedGender, string expectedBranch)
    {
        var request = MemberTestData.ValidAddMember() with { Gender = gender, Branch = branch };
        var validation = new AddMemberHandler(new RejectingMemberDbContext()).Validator.Validate(request);

        var member = request.ToMember();

        Assert.True(validation.IsValid);
        Assert.Equal(expectedGender, member.Gender);
        Assert.Equal(expectedBranch, member.Branch);
        Assert.Equal(gender, request.Gender);
        Assert.Equal(branch, request.Branch);
    }

    [Fact]
    public void ToMember_WhenTextIsPadded_TrimsAllFieldsAndPreservesInternalTextWithoutMutatingInput()
    {
        var request = MemberTestData.ValidAddMember() with
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
        var validation = new AddMemberHandler(new RejectingMemberDbContext()).Validator.Validate(request);

        var member = request.ToMember();

        Assert.True(validation.IsValid);
        Assert.Equal("Élodie  Anne", member.FirstName);
        Assert.Equal("D'Souza-Smith", member.LastName);
        Assert.Equal("009876543210", member.MobileNo);
        Assert.Equal("+91", member.Extension);
        Assert.Equal("alice+alumni@example.com", member.Email);
        Assert.Equal(new Address
        {
            PinCode = "400 001",
            Country = "Côte d'Ivoire",
            State = "Maharashtra",
            City = "Navi  Mumbai",
            UserAddress = "12, Rue de l'Église – Apt.  4"
        }, member.CurrentAddress);
        Assert.Equal(new Address
        {
            PinCode = "SW1A 1AA",
            Country = "United Kingdom",
            State = "Greater London",
            City = "London",
            UserAddress = "34 King's Road, Flat  2"
        }, member.CorrespondenceAddress);
        Assert.Equal(original, request);
        Assert.Same(currentAddress, request.CurrentAddress);
        Assert.Same(correspondenceAddress, request.CorrespondenceAddress);
        Assert.Equal(" 400 001 ", request.CurrentAddress.PinCode);
        Assert.Equal(" 34 King's Road, Flat  2 ", request.CorrespondenceAddress.UserAddress);
    }
}
