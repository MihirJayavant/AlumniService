using Core;
using Xunit;

namespace Alumni.Student.UnitTests;

public sealed class StudentFieldValidationTests
{
    [Theory]
    [InlineData(nameof(AddStudent.FirstName))]
    [InlineData(nameof(AddStudent.LastName))]
    [InlineData(nameof(AddStudent.Extension))]
    [InlineData(nameof(AddStudent.MobileNo))]
    [InlineData(nameof(AddStudent.Gender))]
    [InlineData(nameof(AddStudent.Branch))]
    public async Task Execute_WhenRequiredTextIsMissing_ReturnsBadRequestWithoutDatabaseAccess(string field)
    {
        foreach (var value in new string?[] { null, "", " \t\r\n " })
        {
            await AssertRejected(SetText(StudentTestData.ValidAddStudent(), field, value!), field);
        }
    }

    [Theory]
    [InlineData(nameof(AddStudent.FirstName), 100)]
    [InlineData(nameof(AddStudent.LastName), 100)]
    [InlineData(nameof(AddStudent.Extension), 10)]
    public async Task Validate_WhenTextLengthIsAtBoundary_UsesTrimmedLength(string field, int maximum)
    {
        var value = $" \t{new string('é', maximum)} \r\n";
        var request = SetText(StudentTestData.ValidAddStudent(), field, value);

        AssertValid(request);
        Assert.Equal(value, GetText(request, field));
        await AssertRejected(SetText(request, field, $" {new string('é', maximum + 1)} "), field);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("000000000000000")]
    [InlineData("1234567890123456")]
    [InlineData("+919876543210")]
    [InlineData("987 6543210")]
    [InlineData("987-6543210")]
    [InlineData("１２３４５６７８９０")]
    [InlineData("١٢٣٤٥٦٧٨٩٠")]
    [InlineData("123abc")]
    public async Task Execute_WhenMobileNumberIsInvalid_ReturnsBadRequestWithoutDatabaseAccess(string mobile)
        => await AssertRejected(StudentTestData.ValidAddStudent() with { MobileNo = mobile }, nameof(AddStudent.MobileNo));

    [Theory]
    [InlineData("1")]
    [InlineData("000000000000001")]
    [InlineData("123456789012345")]
    [InlineData(" \t9876543210 \r\n")]
    public void Validate_WhenMobileNumberMeetsPolicy_AcceptsWithoutMutatingInput(string mobile)
    {
        var request = StudentTestData.ValidAddStudent() with { MobileNo = mobile };

        AssertValid(request);
        Assert.Equal(mobile, request.MobileNo);
    }

    [Theory]
    [InlineData(nameof(AddStudent.Gender), "Unknown")]
    [InlineData(nameof(AddStudent.Gender), "F")]
    [InlineData(nameof(AddStudent.Branch), "Mechanical")]
    [InlineData(nameof(AddStudent.Branch), "C S")]
    public async Task Execute_WhenChoiceIsUnsupported_ReturnsBadRequestWithoutDatabaseAccess(string field, string value)
        => await AssertRejected(SetText(StudentTestData.ValidAddStudent(), field, value), field);

    [Fact]
    public async Task Validate_WhenBirthDateIsAtBoundary_RejectsMissingAndFutureDates()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = StudentTestData.ValidAddStudent() with
        {
            DateOfBirth = today,
            AdmissionYear = today.Year,
            PassingYear = today.Year
        };

        AssertValid(request);
        AssertValid(request with { DateOfBirth = DateOnly.MinValue.AddDays(1), AdmissionYear = 1, PassingYear = 1 });
        await AssertRejected(request with { DateOfBirth = DateOnly.MinValue }, nameof(AddStudent.DateOfBirth));
        await AssertRejected(request with { DateOfBirth = today.AddDays(1) }, nameof(AddStudent.DateOfBirth));
    }

    [Theory]
    [InlineData(0, 2020, nameof(AddStudent.AdmissionYear))]
    [InlineData(10000, 2020, nameof(AddStudent.AdmissionYear))]
    [InlineData(2016, 0, nameof(AddStudent.PassingYear))]
    [InlineData(2016, 10000, nameof(AddStudent.PassingYear))]
    [InlineData(2016, 2015, nameof(AddStudent.PassingYear))]
    [InlineData(1997, 2020, nameof(AddStudent.AdmissionYear))]
    public async Task Execute_WhenYearsAreInvalid_ReturnsBadRequestWithoutDatabaseAccess(int admission, int passing, string field)
        => await AssertRejected(StudentTestData.ValidAddStudent() with { AdmissionYear = admission, PassingYear = passing }, field);

    [Theory]
    [InlineData(1998, 1998)]
    [InlineData(9999, 9999)]
    public void Validate_WhenYearsMeetBoundsAndChronology_Accepts(int admission, int passing)
        => AssertValid(StudentTestData.ValidAddStudent() with { AdmissionYear = admission, PassingYear = passing });

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Execute_WhenAddressIsNull_ReturnsBadRequestWithoutDatabaseAccess(bool current)
    {
        var request = StudentTestData.ValidAddStudent();
        request = current ? request with { CurrentAddress = null! } : request with { CorrespondenceAddress = null! };

        await AssertRejected(request, current ? nameof(AddStudent.CurrentAddress) : nameof(AddStudent.CorrespondenceAddress));
    }

    [Theory]
    [InlineData(nameof(Address.PinCode))]
    [InlineData(nameof(Address.Country))]
    [InlineData(nameof(Address.State))]
    [InlineData(nameof(Address.City))]
    [InlineData(nameof(Address.UserAddress))]
    public async Task Execute_WhenAddressFieldIsMissing_ReturnsBadRequestWithoutDatabaseAccess(string field)
    {
        foreach (var current in new[] { true, false })
        {
            foreach (var value in new string?[] { null, "", " \t\r\n " })
            {
                await AssertRejected(SetAddressText(StudentTestData.ValidAddStudent(), current, field, value!),
                    $"{(current ? nameof(AddStudent.CurrentAddress) : nameof(AddStudent.CorrespondenceAddress))}.{field}");
            }
        }
    }

    [Theory]
    [InlineData(nameof(Address.Country))]
    [InlineData(nameof(Address.State))]
    [InlineData(nameof(Address.City))]
    [InlineData(nameof(Address.UserAddress))]
    public async Task Validate_WhenAddressLengthIsAtBoundary_UsesEachAddressLimitAfterTrimming(string field)
    {
        foreach (var current in new[] { true, false })
        {
            var maximum = current || field == nameof(Address.UserAddress) ? 100 : 30;
            var request = SetAddressText(StudentTestData.ValidAddStudent(), current, field, $" \t{new string('é', maximum)} \n");

            AssertValid(request);
            await AssertRejected(SetAddressText(request, current, field, $" {new string('é', maximum + 1)} "),
                $"{(current ? nameof(AddStudent.CurrentAddress) : nameof(AddStudent.CorrespondenceAddress))}.{field}");
        }
    }

    [Fact]
    public void Validate_WhenPinCodeIsNonempty_DoesNotImposeFormatOrLengthRules()
    {
        var request = StudentTestData.ValidAddStudent();
        request = SetAddressText(request, true, nameof(Address.PinCode), $" {new string('x', 200)} ");
        request = SetAddressText(request, false, nameof(Address.PinCode), " SW1A 1AA ");

        AssertValid(request);
    }

    private static void AssertValid(AddStudent request)
    {
        var context = new RejectingStudentDbContext();
        var result = new AddStudentHandler(context).Validator.Validate(request);

        Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(error => error.ErrorMessage)));
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    private static async Task AssertRejected(AddStudent request, string propertyName)
    {
        var context = new RejectingStudentDbContext();
        var handler = new AddStudentHandler(context);
        var validation = handler.Validator.Validate(request);

        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.PropertyName == propertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    private static AddStudent SetText(AddStudent request, string field, string value) => field switch
    {
        nameof(AddStudent.FirstName) => request with { FirstName = value },
        nameof(AddStudent.LastName) => request with { LastName = value },
        nameof(AddStudent.Extension) => request with { Extension = value },
        nameof(AddStudent.MobileNo) => request with { MobileNo = value },
        nameof(AddStudent.Gender) => request with { Gender = value },
        nameof(AddStudent.Branch) => request with { Branch = value },
        _ => throw new ArgumentOutOfRangeException(nameof(field))
    };

    private static string GetText(AddStudent request, string field) => field switch
    {
        nameof(AddStudent.FirstName) => request.FirstName,
        nameof(AddStudent.LastName) => request.LastName,
        nameof(AddStudent.Extension) => request.Extension,
        _ => throw new ArgumentOutOfRangeException(nameof(field))
    };

    private static AddStudent SetAddressText(AddStudent request, bool current, string field, string value)
    {
        var address = current ? request.CurrentAddress : request.CorrespondenceAddress;
        address = field switch
        {
            nameof(Address.PinCode) => address with { PinCode = value },
            nameof(Address.Country) => address with { Country = value },
            nameof(Address.State) => address with { State = value },
            nameof(Address.City) => address with { City = value },
            nameof(Address.UserAddress) => address with { UserAddress = value },
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };
        return current ? request with { CurrentAddress = address } : request with { CorrespondenceAddress = address };
    }
}
