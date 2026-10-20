using Core;
using Xunit;

namespace Alumni.Network.UnitTests;

public sealed class MemberFieldValidationTests
{
    [Theory]
    [InlineData(nameof(AddMember.FirstName))]
    [InlineData(nameof(AddMember.LastName))]
    [InlineData(nameof(AddMember.Extension))]
    [InlineData(nameof(AddMember.MobileNo))]
    [InlineData(nameof(AddMember.Gender))]
    [InlineData(nameof(AddMember.Branch))]
    public async Task Execute_WhenRequiredTextIsMissing_ReturnsBadRequestWithoutDatabaseAccess(string field)
    {
        foreach (var value in new string?[] { null, "", " \t\r\n " })
        {
            await AssertRejected(SetText(MemberTestData.ValidAddMember(), field, value!), field);
        }
    }

    [Theory]
    [InlineData(nameof(AddMember.FirstName), 100)]
    [InlineData(nameof(AddMember.LastName), 100)]
    [InlineData(nameof(AddMember.Extension), 10)]
    [InlineData(nameof(AddMember.Branch), 30)]
    public async Task Validate_WhenTextLengthIsAtBoundary_UsesTrimmedLength(string field, int maximum)
    {
        var value = $" \t{new string('é', maximum)} \r\n";
        var request = SetText(MemberTestData.ValidAddMember(), field, value);

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
        => await AssertRejected(MemberTestData.ValidAddMember() with { MobileNo = mobile }, nameof(AddMember.MobileNo));

    [Theory]
    [InlineData("1")]
    [InlineData("000000000000001")]
    [InlineData("123456789012345")]
    [InlineData(" \t9876543210 \r\n")]
    public void Validate_WhenMobileNumberMeetsPolicy_AcceptsWithoutMutatingInput(string mobile)
    {
        var request = MemberTestData.ValidAddMember() with { MobileNo = mobile };

        AssertValid(request);
        Assert.Equal(mobile, request.MobileNo);
    }

    [Theory]
    [InlineData(nameof(AddMember.Gender), "Unknown")]
    [InlineData(nameof(AddMember.Gender), "F")]
    public async Task Execute_WhenChoiceIsUnsupported_ReturnsBadRequestWithoutDatabaseAccess(string field, string value)
        => await AssertRejected(SetText(MemberTestData.ValidAddMember(), field, value), field);

    [Fact]
    public async Task Validate_WhenBirthDateIsAtBoundary_EnforcesAgeLimits()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = MemberTestData.ValidAddMember() with { DateOfBirth = today.AddYears(-100) };

        AssertValid(request);
        AssertValid(request with { DateOfBirth = today.AddYears(-10).AddDays(-1) });
        await AssertRejected(request with { DateOfBirth = today.AddYears(-100).AddDays(-1) }, nameof(AddMember.DateOfBirth));
        await AssertRejected(request with { DateOfBirth = today.AddYears(-10) }, nameof(AddMember.DateOfBirth));
        await AssertRejected(request with { DateOfBirth = DateOnly.MinValue }, nameof(AddMember.DateOfBirth));
        await AssertRejected(request with { DateOfBirth = today.AddDays(1) }, nameof(AddMember.DateOfBirth));
    }

    [Theory]
    [InlineData(0, 2020, nameof(AddMember.AdmissionYear))]
    [InlineData(10000, 2020, nameof(AddMember.AdmissionYear))]
    [InlineData(2016, 0, nameof(AddMember.PassingYear))]
    [InlineData(2016, 10000, nameof(AddMember.PassingYear))]
    [InlineData(2016, 2015, nameof(AddMember.PassingYear))]
    [InlineData(1997, 2020, nameof(AddMember.AdmissionYear))]
    public async Task Execute_WhenYearsAreInvalid_ReturnsBadRequestWithoutDatabaseAccess(int admission, int passing, string field)
        => await AssertRejected(MemberTestData.ValidAddMember() with { AdmissionYear = admission, PassingYear = passing }, field);

    [Theory]
    [InlineData(1998, 1998)]
    public void Validate_WhenYearsMeetBoundsAndChronology_Accepts(int admission, int passing)
        => AssertValid(MemberTestData.ValidAddMember() with { AdmissionYear = admission, PassingYear = passing });

    [Fact]
    public async Task Validate_WhenYearIsCurrentOrFuture_EnforcesCurrentYearLimit()
    {
        var year = DateTime.UtcNow.Year;
        var request = MemberTestData.ValidAddMember() with { AdmissionYear = year, PassingYear = year };

        AssertValid(request);
        await AssertRejected(request with { AdmissionYear = year + 1, PassingYear = year + 1 }, nameof(AddMember.AdmissionYear));
        await AssertRejected(request with { PassingYear = year + 1 }, nameof(AddMember.PassingYear));
        await AssertRejected(request with { AdmissionYear = 1899 }, nameof(AddMember.AdmissionYear));
    }

    [Theory]
    [InlineData("Mechanical")]
    [InlineData("Civil Engineering")]
    [InlineData(" C S ")]
    public void Validate_WhenBranchIsOutsideListedConstants_AcceptsNonemptyDomainText(string branch)
        => AssertValid(MemberTestData.ValidAddMember() with { Branch = branch });

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Execute_WhenAddressIsNull_ReturnsBadRequestWithoutDatabaseAccess(bool current)
    {
        var request = MemberTestData.ValidAddMember();
        request = current ? request with { CurrentAddress = null! } : request with { CorrespondenceAddress = null! };

        await AssertRejected(request, current ? nameof(AddMember.CurrentAddress) : nameof(AddMember.CorrespondenceAddress));
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
                await AssertRejected(SetAddressText(MemberTestData.ValidAddMember(), current, field, value!),
                    $"{(current ? nameof(AddMember.CurrentAddress) : nameof(AddMember.CorrespondenceAddress))}.{field}");
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
            const int maximum = 100;
            var request = SetAddressText(MemberTestData.ValidAddMember(), current, field, $" \t{new string('é', maximum)} \n");

            AssertValid(request);
            await AssertRejected(SetAddressText(request, current, field, $" {new string('é', maximum + 1)} "),
                $"{(current ? nameof(AddMember.CurrentAddress) : nameof(AddMember.CorrespondenceAddress))}.{field}");
        }
    }

    [Fact]
    public void Validate_WhenPinCodeIsNonempty_DoesNotImposeFormatOrLengthRules()
    {
        var request = MemberTestData.ValidAddMember();
        request = SetAddressText(request, true, nameof(Address.PinCode), $" {new string('x', 200)} ");
        request = SetAddressText(request, false, nameof(Address.PinCode), " SW1A 1AA ");

        AssertValid(request);
    }

    private static void AssertValid(AddMember request)
    {
        var context = new RejectingMemberDbContext();
        var result = new AddMemberHandler(context).Validator.Validate(request);

        Assert.True(result.IsValid, string.Join("; ", result.Errors.Select(error => error.ErrorMessage)));
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    private static async Task AssertRejected(AddMember request, string propertyName)
    {
        var context = new RejectingMemberDbContext();
        var handler = new AddMemberHandler(context);
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

    private static AddMember SetText(AddMember request, string field, string value) => field switch
    {
        nameof(AddMember.FirstName) => request with { FirstName = value },
        nameof(AddMember.LastName) => request with { LastName = value },
        nameof(AddMember.Extension) => request with { Extension = value },
        nameof(AddMember.MobileNo) => request with { MobileNo = value },
        nameof(AddMember.Gender) => request with { Gender = value },
        nameof(AddMember.Branch) => request with { Branch = value },
        _ => throw new ArgumentOutOfRangeException(nameof(field))
    };

    private static string GetText(AddMember request, string field) => field switch
    {
        nameof(AddMember.FirstName) => request.FirstName,
        nameof(AddMember.LastName) => request.LastName,
        nameof(AddMember.Extension) => request.Extension,
        nameof(AddMember.Branch) => request.Branch,
        _ => throw new ArgumentOutOfRangeException(nameof(field))
    };

    private static AddMember SetAddressText(AddMember request, bool current, string field, string value)
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
