using Core;
using Xunit;

namespace Alumni.Faculty.UnitTests;

public sealed class AddFacultyValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n ")]
    [InlineData("faculty")]
    [InlineData("faculty@")]
    [InlineData("@example.com")]
    [InlineData("faculty@example")]
    [InlineData("faculty..member@example.com")]
    [InlineData("faculty member@example.com")]
    [InlineData("faculty@-example.com")]
    public void Validate_WhenEmailIsMissingOrMalformed_RejectsEmail(string? email)
    {
        var request = FacultyTestData.ValidAddFaculty() with { Email = email! };

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Equal(nameof(AddFaculty.Email), Assert.Single(result.Errors).PropertyName);
    }

    [Theory]
    [InlineData("Faculty.Member@Example.COM")]
    [InlineData("  Faculty.Member@Example.COM \t")]
    [InlineData("faculty.member+alumni@example.com")]
    [InlineData("faculty@xn--bcher-kva.example")]
    public void Validate_WhenEmailMeetsPolicy_AcceptsRequest(string email)
    {
        var request = FacultyTestData.ValidAddFaculty() with { Email = email };

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal(email, request.Email);
    }

    [Theory]
    [InlineData(100, false, true)]
    [InlineData(100, true, true)]
    [InlineData(101, false, false)]
    [InlineData(101, true, false)]
    public void Validate_WhenEmailLengthIsAtBoundary_UsesTrimmedLength(int length, bool padded, bool valid)
    {
        // Both addresses satisfy Core's syntax rules; only Faculty's length limit differs.
        var email = new string('a', 64) + "@" + new string('b', length - 69) + ".com";
        _ = new Email(email);
        var request = FacultyTestData.ValidAddFaculty() with { Email = padded ? $"  {email} \t" : email };

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.Equal(valid, result.IsValid);
        if (!valid)
        {
            var error = Assert.Single(result.Errors);
            Assert.Equal(nameof(AddFaculty.Email), error.PropertyName);
            Assert.Equal("Email must be at most 100 characters.", error.ErrorMessage);
        }
    }

    [Theory]
    [InlineData(nameof(AddFaculty.FirstName), null)]
    [InlineData(nameof(AddFaculty.FirstName), "")]
    [InlineData(nameof(AddFaculty.FirstName), " \t\r\n ")]
    [InlineData(nameof(AddFaculty.LastName), null)]
    [InlineData(nameof(AddFaculty.LastName), "")]
    [InlineData(nameof(AddFaculty.LastName), " \t\r\n ")]
    [InlineData(nameof(AddFaculty.Extension), null)]
    [InlineData(nameof(AddFaculty.Extension), "")]
    [InlineData(nameof(AddFaculty.Extension), " \t\r\n ")]
    public void Validate_WhenRequiredTextIsMissing_RejectsOnlyThatField(string property, string? value)
    {
        var request = WithText(FacultyTestData.ValidAddFaculty(), property, value!);

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Equal(property, Assert.Single(result.Errors).PropertyName);
    }

    [Theory]
    [InlineData(nameof(AddFaculty.FirstName), 100, false, true)]
    [InlineData(nameof(AddFaculty.FirstName), 100, true, true)]
    [InlineData(nameof(AddFaculty.FirstName), 101, false, false)]
    [InlineData(nameof(AddFaculty.FirstName), 101, true, false)]
    [InlineData(nameof(AddFaculty.LastName), 100, false, true)]
    [InlineData(nameof(AddFaculty.LastName), 100, true, true)]
    [InlineData(nameof(AddFaculty.LastName), 101, false, false)]
    [InlineData(nameof(AddFaculty.LastName), 101, true, false)]
    [InlineData(nameof(AddFaculty.Extension), 10, false, true)]
    [InlineData(nameof(AddFaculty.Extension), 10, true, true)]
    [InlineData(nameof(AddFaculty.Extension), 11, false, false)]
    [InlineData(nameof(AddFaculty.Extension), 11, true, false)]
    public void Validate_WhenTextLengthIsAtBoundary_UsesTrimmedLength(string property, int length, bool padded, bool valid)
    {
        var value = new string('a', length);
        var request = WithText(FacultyTestData.ValidAddFaculty(), property, padded ? $" \t{value}  " : value);

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.Equal(valid, result.IsValid);
        if (!valid)
        {
            var error = Assert.Single(result.Errors);
            Assert.Equal(property, error.PropertyName);
            var maximumLength = property == nameof(AddFaculty.Extension) ? 10 : 100;
            Assert.Equal($"{property} must be at most {maximumLength} characters.", error.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("Élodie", "O'Connor-Smith")]
    [InlineData("  李  ", " 王 ")]
    [InlineData("Anne  Marie", "de la Cruz")]
    [InlineData("आशा", "देशमुख")]
    public void Validate_WhenNamesContainUnicodePunctuationOrInternalSpaces_AcceptsWithoutMutatingInput(string firstName, string lastName)
    {
        var request = FacultyTestData.ValidAddFaculty() with { FirstName = firstName, LastName = lastName };

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal(firstName, request.FirstName);
        Assert.Equal(lastName, request.LastName);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(long.MinValue)]
    public void Validate_WhenMobileNumberIsNotPositive_RejectsMobileNumber(long mobileNo)
    {
        var request = FacultyTestData.ValidAddFaculty() with { MobileNo = mobileNo };

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Equal(nameof(AddFaculty.MobileNo), Assert.Single(result.Errors).PropertyName);
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(9876543210L)]
    [InlineData(long.MaxValue)]
    public void Validate_WhenMobileNumberIsPositive_AcceptsRequest(long mobileNo)
    {
        var request = FacultyTestData.ValidAddFaculty() with { MobileNo = mobileNo };

        var result = new AddFacultyHandler(new RejectingFacultyDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(nameof(AddFaculty.Email), "not-an-email", "Email must be a valid email address.")]
    [InlineData(nameof(AddFaculty.FirstName), null, "FirstName must be at most 100 characters.")]
    [InlineData(nameof(AddFaculty.LastName), null, "LastName must be at most 100 characters.")]
    [InlineData(nameof(AddFaculty.Extension), null, "Extension must be at most 10 characters.")]
    public async Task Execute_WhenRequestIsInvalid_ReturnsBadRequestWithoutDatabaseAccess(string property, string? value, string expectedMessage)
    {
        var invalidValue = value ?? new string('a', property == nameof(AddFaculty.Extension) ? 11 : 101);
        var request = WithText(FacultyTestData.ValidAddFaculty(), property, invalidValue);
        var context = new RejectingFacultyDbContext();
        var handler = new AddFacultyHandler(context);

        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(expectedMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    private static AddFaculty WithText(AddFaculty request, string property, string value) => property switch
    {
        nameof(AddFaculty.Email) => request with { Email = value },
        nameof(AddFaculty.FirstName) => request with { FirstName = value },
        nameof(AddFaculty.LastName) => request with { LastName = value },
        nameof(AddFaculty.Extension) => request with { Extension = value },
        _ => throw new ArgumentOutOfRangeException(nameof(property), property, "Unsupported Faculty text property.")
    };
}
