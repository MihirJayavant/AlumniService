using Xunit;

namespace Core.UnitTests;

public class EmailTests
{
    [Theory]
    [InlineData("alumni@example.com")]
    [InlineData("alumni@students.example.com")]
    [InlineData("alumni+updates@example.com")]
    public void Constructor_WhenAddressIsValid_PreservesValue(string value)
    {
        var email = new Email(value);

        Assert.Equal(value, email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\n")]
    [InlineData("alumni.example.com")]
    [InlineData("alumni@example")]
    [InlineData("@example.com")]
    [InlineData("alumni@")]
    [InlineData("alumni@@example.com")]
    [InlineData("alumni student@example.com")]
    [InlineData("alumni@exam ple.com")]
    [InlineData(" alumni@example.com")]
    [InlineData("alumni@example.com ")]
    public void Constructor_WhenAddressIsInvalid_ThrowsArgumentExceptionForValue(string? value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Email(value!));

        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void ImplicitConversion_WhenAddressIsValid_RoundTripsValue()
    {
        const string value = "alumni+updates@example.com";

        Email email = value;
        string convertedValue = email;

        Assert.Equal(value, email.Value);
        Assert.Equal(value, convertedValue);
    }

    [Fact]
    public void ImplicitConversion_WhenAddressIsInvalid_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            _ = (Email)"invalid-address";
        });

        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void ToString_WhenCalled_ReturnsAddress()
    {
        var email = new Email("alumni@example.com");

        Assert.Equal("alumni@example.com", email.ToString());
    }

    [Fact]
    public void Equals_WhenAddressesMatch_ReturnsTrue()
    {
        var first = new Email("alumni@example.com");
        var second = new Email("alumni@example.com");

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void Equals_WhenAddressesDiffer_ReturnsFalse()
    {
        var first = new Email("alumni@example.com");
        var second = new Email("faculty@example.com");

        Assert.NotEqual(first, second);
        Assert.True(first != second);
    }
}
