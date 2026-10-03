using FluentValidation;
using Xunit;

namespace Core.UnitTests;

public sealed class ValidationExtensionsTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" \t ", false)]
    [InlineData("  Élodie  ", true)]
    [InlineData("12345678901", false)]
    public void RequiredText_WhenInputIsMissingOrAtLengthLimit_UsesTrimmedLength(string? text, bool valid)
    {
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.Text).RequiredText(10);
        var request = new Request { Text = text! };

        var result = validator.Validate(request);

        Assert.Equal(valid, result.IsValid);
        Assert.Equal(text, request.Text);
        if (!valid)
        {
            Assert.Equal(nameof(Request.Text), Assert.Single(result.Errors).PropertyName);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(" \t ", false)]
    [InlineData("0", false)]
    [InlineData("000000000000000", false)]
    [InlineData("1", true)]
    [InlineData(" 000000000000001 ", true)]
    [InlineData("1234567890123456", false)]
    [InlineData("+919876543210", false)]
    [InlineData("987 6543210", false)]
    [InlineData("１２３４５", false)]
    public void ValidMobileNumber_WhenInputVaries_EnforcesDigitPolicy(string? mobile, bool valid)
    {
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.Text).ValidMobileNumber();

        var result = validator.Validate(new Request { Text = mobile! });

        Assert.Equal(valid, result.IsValid);
        if (!valid)
        {
            Assert.Equal(nameof(Request.Text), Assert.Single(result.Errors).PropertyName);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" \t ", false)]
    [InlineData(" Male ", true)]
    [InlineData("fEmAlE", true)]
    [InlineData("Unknown", false)]
    public void ValidGender_WhenInputVaries_AcceptsTrimmedCaseInsensitiveValues(string? gender, bool valid)
    {
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.Text).ValidGender();

        var result = validator.Validate(new Request { Text = gender! });

        Assert.Equal(valid, result.IsValid);
        if (!valid)
        {
            Assert.Equal(nameof(Request.Text), Assert.Single(result.Errors).PropertyName);
        }
    }

    [Theory]
    [InlineData(1899, false)]
    [InlineData(1900, true)]
    [InlineData(2026, true)]
    [InlineData(2027, false)]
    public void ValidYear_WhenAtBoundaries_UsesCurrentUtcYear(int year, bool valid)
    {
        var clock = new TestTimeProvider(new DateTimeOffset(2026, 10, 3, 0, 0, 0, TimeSpan.Zero));
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.Year).ValidYear(timeProvider: clock);

        Assert.Equal(valid, validator.Validate(new Request { Year = year }).IsValid);
    }

    [Fact]
    public void ValidYear_WhenClockAdvances_ReevaluatesYearWithoutRecreatingValidator()
    {
        var clock = new TestTimeProvider(new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero));
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.Year).ValidYear(minimumYear: 2000, timeProvider: clock);

        Assert.False(validator.Validate(new Request { Year = 1999 }).IsValid);
        Assert.True(validator.Validate(new Request { Year = 2000 }).IsValid);
        Assert.False(validator.Validate(new Request { Year = 2027 }).IsValid);
        clock.UtcNow = clock.UtcNow.AddSeconds(1);
        Assert.True(validator.Validate(new Request { Year = 2027 }).IsValid);
    }

    [Theory]
    [InlineData(100, -1, false)]
    [InlineData(100, 0, true)]
    [InlineData(10, -1, true)]
    [InlineData(10, 0, false)]
    [InlineData(10, 1, false)]
    [InlineData(0, 0, false)]
    public void ValidDateOfBirth_WhenAtAgeBoundaries_EnforcesExclusiveMinimumAndInclusiveMaximum(
        int yearsAgo, int daysOffset, bool valid)
    {
        var today = new DateOnly(2026, 10, 3);
        var clock = new TestTimeProvider(new DateTimeOffset(2026, 10, 3, 0, 0, 0, TimeSpan.Zero));
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.BirthDate).ValidDateOfBirth(timeProvider: clock);

        var result = validator.Validate(new Request { BirthDate = today.AddYears(-yearsAgo).AddDays(daysOffset) });

        Assert.Equal(valid, result.IsValid);
    }

    [Fact]
    public void ValidDateOfBirth_WhenDateIsDefaultOrFuture_RejectsInput()
    {
        var clock = new TestTimeProvider(new DateTimeOffset(2026, 10, 3, 0, 0, 0, TimeSpan.Zero));
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.BirthDate).ValidDateOfBirth(timeProvider: clock);

        Assert.False(validator.Validate(new Request { BirthDate = default }).IsValid);
        Assert.False(validator.Validate(new Request { BirthDate = new DateOnly(2026, 10, 4) }).IsValid);
    }

    [Fact]
    public void ValidDateOfBirth_WhenTodayIsLeapDay_UsesCalendarAnniversariesAndUpdatesWithClock()
    {
        var clock = new TestTimeProvider(new DateTimeOffset(2024, 2, 28, 0, 0, 0, TimeSpan.Zero));
        var validator = new InlineValidator<Request>();
        validator.RuleFor(x => x.BirthDate).ValidDateOfBirth(timeProvider: clock);

        Assert.True(validator.Validate(new Request { BirthDate = new DateOnly(1924, 2, 29) }).IsValid);
        Assert.True(validator.Validate(new Request { BirthDate = new DateOnly(1924, 2, 28) }).IsValid);
        var request = new Request { BirthDate = new DateOnly(2014, 2, 28) };
        Assert.False(validator.Validate(request).IsValid);
        clock.UtcNow = clock.UtcNow.AddDays(1);
        Assert.True(validator.Validate(request).IsValid);
        Assert.False(validator.Validate(new Request { BirthDate = new DateOnly(1924, 2, 28) }).IsValid);
    }

    private sealed record Request
    {
        public string Text { get; init; } = "";
        public int Year { get; init; }
        public DateOnly BirthDate { get; init; }
    }

    private sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public DateTimeOffset UtcNow { get; set; } = utcNow;
        public override DateTimeOffset GetUtcNow() => UtcNow;
    }
}
