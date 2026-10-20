using Alumni.Network.Companies;
using Core;
using Xunit;

namespace Alumni.Network.UnitTests.Companies;

public sealed class CompanyValidationTests
{
    [Theory]
    [InlineData("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1")]
    [InlineData("01924023-91e0-7000-8000-000000000001")]
    public void Validate_WhenMemberIdIsNonEmpty_AcceptsAddAndGet(string identifier)
    {
        var memberId = Guid.Parse(identifier);
        var context = new RejectingMemberDbContext();

        var addResult = new AddCompanyHandler(context).Validator.Validate(ValidAddCompany() with { MemberId = memberId });
        var getResult = new GetCompanyHandler(context).Validator.Validate(new GetCompany { MemberId = memberId });

        Assert.True(addResult.IsValid);
        Assert.Empty(addResult.Errors);
        Assert.True(getResult.IsValid);
        Assert.Empty(getResult.Errors);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenAddMemberIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingMemberDbContext();
        var handler = new AddCompanyHandler(context);
        var request = ValidAddCompany() with { MemberId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(AddCompany.MemberId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Fact]
    public async Task Execute_WhenGetMemberIdIsEmpty_ReturnsBadRequestWithoutDatabaseAccess()
    {
        var context = new RejectingMemberDbContext();
        var handler = new GetCompanyHandler(context);
        var request = new GetCompany { MemberId = Guid.Empty };

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(nameof(GetCompany.MemberId), Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(nameof(AddCompany.CompanyName), null)]
    [InlineData(nameof(AddCompany.CompanyName), "")]
    [InlineData(nameof(AddCompany.CompanyName), " \t\r\n ")]
    [InlineData(nameof(AddCompany.Designation), null)]
    [InlineData(nameof(AddCompany.Designation), "")]
    [InlineData(nameof(AddCompany.Designation), " \t\r\n ")]
    public async Task Execute_WhenRequiredTextIsMissing_ReturnsBadRequestWithoutDatabaseAccess(string property, string? value)
    {
        var request = property == nameof(AddCompany.CompanyName)
            ? ValidAddCompany() with { CompanyName = value! }
            : ValidAddCompany() with { Designation = value! };
        var context = new RejectingMemberDbContext();
        var handler = new AddCompanyHandler(context);

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Equal(property, Assert.Single(validation.Errors).PropertyName);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(nameof(AddCompany.CompanyName), 50)]
    [InlineData(nameof(AddCompany.Designation), 30)]
    public async Task Execute_WhenTrimmedTextExceedsLimit_ReturnsBadRequestWithoutDatabaseAccess(string property, int maximumLength)
        => await AssertRejected(SetText(property, "  " + new string('a', maximumLength + 1) + "  "), property);

    [Theory]
    [InlineData(nameof(AddCompany.CompanyName), 50)]
    [InlineData(nameof(AddCompany.Designation), 30)]
    public void Validate_WhenTrimmedTextIsAtLimit_AcceptsWithoutMutatingRequest(string property, int maximumLength)
    {
        var value = "  " + new string('a', maximumLength) + "  ";
        var request = SetText(property, value);

        var result = new AddCompanyHandler(new RejectingMemberDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        var actual = property switch
        {
            nameof(AddCompany.CompanyName) => request.CompanyName,
            nameof(AddCompany.Designation) => request.Designation,
            _ => throw new ArgumentOutOfRangeException(nameof(property))
        };
        Assert.Equal(value, actual);
    }

    private static AddCompany SetText(string property, string? value) => property switch
    {
        nameof(AddCompany.CompanyName) => ValidAddCompany() with { CompanyName = value! },
        nameof(AddCompany.Designation) => ValidAddCompany() with { Designation = value! },
        _ => throw new ArgumentOutOfRangeException(nameof(property))
    };

    private static async Task AssertRejected(AddCompany request, string property)
    {
        var context = new RejectingMemberDbContext();
        var handler = new AddCompanyHandler(context);

        var validation = handler.Validator.Validate(request);
        var result = await handler.Execute(request, TestContext.Current.CancellationToken);

        Assert.Contains(validation.Errors, error => error.PropertyName == property);
        Assert.True(result.IsT1);
        Assert.Equal(ResponseStatus.BadRequest, result.AsT1.Status);
        Assert.Equal(validation.Errors[0].ErrorMessage, result.AsT1.Message);
        Assert.Equal(0, context.QueryAccessCount);
        Assert.Equal(0, context.SaveChangesCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1899)]
    [InlineData(10000)]
    public async Task Execute_WhenYearOfJoiningIsOutOfRange_ReturnsBadRequestWithoutDatabaseAccess(int value)
        => await AssertRejected(ValidAddCompany() with { YearOfJoining = value }, nameof(AddCompany.YearOfJoining));

    [Theory]
    [InlineData(1900)]
    [InlineData(2020)]
    public void Validate_WhenYearOfJoiningIsAtBoundary_Accepts(int value)
    {
        var request = ValidAddCompany() with { YearOfJoining = value };

        var result = new AddCompanyHandler(new RejectingMemberDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Execute_WhenSalaryIsNegative_ReturnsBadRequestWithoutDatabaseAccess()
        => await AssertRejected(ValidAddCompany() with { AnnualSalary = -1 }, nameof(AddCompany.AnnualSalary));

    [Theory]
    [InlineData(0L)]
    [InlineData(long.MaxValue)]
    public void Validate_WhenSalaryIsNonNegative_Accepts(long salary)
    {
        var request = ValidAddCompany() with { AnnualSalary = salary };

        var result = new AddCompanyHandler(new RejectingMemberDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WhenYearIsCurrentOrFuture_EnforcesCurrentYearLimit()
    {
        var year = DateTime.UtcNow.Year;
        var request = ValidAddCompany() with { YearOfJoining = year };
        var result = new AddCompanyHandler(new RejectingMemberDbContext()).Validator.Validate(request);

        Assert.True(result.IsValid);
        await AssertRejected(request with { YearOfJoining = year + 1 }, nameof(AddCompany.YearOfJoining));
    }

    private static AddCompany ValidAddCompany() => new()
    {
        MemberId = Guid.Parse("b1da6b5b-dc83-45ca-af05-9bf98b0b99d1"),
        CompanyId = Guid.Parse("cf3a4b49-7b78-4bd2-b51c-5ea3a428f99a"),
        CompanyName = "Alumni Systems",
        Designation = "Software Engineer",
        YearOfJoining = 2023,
        AnnualSalary = 1234567
    };
}
