using Xunit;

namespace Core.UnitTests;

public class EmailTests
{
    [Theory]
    [InlineData("alumni@example.com")]
    [InlineData("alumni@students.example.com")]
    [InlineData("alumni+updates@example.com")]
    [InlineData("first.last@example.com")]
    [InlineData("o'connor@example.com")]
    [InlineData("alumni_test@example-domain.com")]
    [InlineData("alumni@x.io")]
    [InlineData("alumni@xn--bcher-kva.example")]
    [InlineData("!#$%&'*+-/=?^_`{|}~@example.com")]
    public void Constructor_WhenAddressIsValid_PreservesValue(string value)
    {
        var email = new Email(value);

        Assert.Equal(value, email.Value);
    }

    [Theory]
    [InlineData(" alumni@example.com", "alumni@example.com")]
    [InlineData("alumni@example.com ", "alumni@example.com")]
    [InlineData("\t ALUMNI@EXAMPLE.COM \r\n", "alumni@example.com")]
    [InlineData("Alumni+Updates@Students.Example.Com", "alumni+updates@students.example.com")]
    public void Constructor_WhenAddressHasWhitespaceOrUppercase_NormalizesValue(string value, string expected)
    {
        var email = new Email(value);

        Assert.Equal(expected, email.Value);
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
    [InlineData(" invalid-address ")]
    [InlineData(" ALUMNI@EXAM PLE.COM ")]
    [InlineData(".alumni@example.com")]
    [InlineData("alumni.@example.com")]
    [InlineData("alumni..student@example.com")]
    [InlineData("alumni@.example.com")]
    [InlineData("alumni@example..com")]
    [InlineData("alumni@example.com.")]
    [InlineData("alumni@-example.com")]
    [InlineData("alumni@example-.com")]
    [InlineData("alumni@example.-com")]
    [InlineData("alumni@example.com-")]
    [InlineData("alumni@example_.com")]
    [InlineData("alumni@example.com>")]
    [InlineData("alumni\0@example.com")]
    [InlineData("alumni@exam\0ple.com")]
    [InlineData("alumni\nstudent@example.com")]
    [InlineData("alumni@exam\rple.com")]
    [InlineData("alumni,student@example.com")]
    [InlineData("Alumni <alumni@example.com>")]
    [InlineData("alumni@example.com,faculty@example.com")]
    public void Constructor_WhenAddressIsInvalid_ThrowsArgumentExceptionForValue(string? value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Email(value!));

        Assert.Equal("value", exception.ParamName);
    }

    [Theory]
    [InlineData(63, true)]
    [InlineData(64, true)]
    [InlineData(65, false)]
    public void Constructor_WhenLocalPartLengthChanges_EnforcesLimit(int length, bool valid)
    {
        var value = new string('a', length) + "@example.com";

        if (valid)
        {
            Assert.Equal(value, new Email(value).Value);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new Email(value));
        }
    }

    [Theory]
    [InlineData("\"alumni\"@example.com")]
    [InlineData("alumni@[127.0.0.1]")]
    [InlineData("élève@example.com")]
    [InlineData("alumni@bücher.example")]
    public void Constructor_WhenAddressUsesUnsupportedSyntax_ThrowsArgumentException(string value)
        => Assert.Throws<ArgumentException>(() => new Email(value));

    [Theory]
    [InlineData(1, true)]
    [InlineData(63, true)]
    [InlineData(64, false)]
    public void Constructor_WhenDomainLabelLengthChanges_EnforcesLimit(int length, bool valid)
    {
        var value = "alumni@" + new string('a', length) + ".example";

        if (valid)
        {
            Assert.Equal(value, new Email(value).Value);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new Email(value));
        }
    }

    [Theory]
    [InlineData(61, true)]
    [InlineData(62, false)]
    public void Constructor_WhenAddressLengthChanges_EnforcesLimit(int finalLabelLength, bool valid)
    {
        // 64-character local part plus 189-character domain gives 254 characters.
        var value = new string('a', 64) + "@" + new string('b', 63)
            + "." + new string('c', 63) + "." + new string('d', finalLabelLength);

        if (valid)
        {
            Assert.Equal(value, new Email(value).Value);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new Email(value));
        }
    }

    [Fact]
    public void ImplicitConversion_WhenAddressHasWhitespaceOrUppercase_ReturnsNormalizedValue()
    {
        const string value = " Alumni+Updates@Example.Com ";

        Email email = value;
        string convertedValue = email;

        Assert.Equal("alumni+updates@example.com", email.Value);
        Assert.Equal("alumni+updates@example.com", convertedValue);
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
        var email = new Email(" Alumni@Example.Com ");

        Assert.Equal("alumni@example.com", email.ToString());
    }

    [Fact]
    public void Equals_WhenNormalizedAddressesMatch_ReturnsTrue()
    {
        var first = new Email("alumni@example.com");
        var second = new Email(" ALUMNI@EXAMPLE.COM ");

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
