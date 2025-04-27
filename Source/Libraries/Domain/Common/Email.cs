using System.Text.RegularExpressions;

namespace Domain.Common;

public readonly partial record struct Email
{
    public string Value { get; } // Encapsulated email value

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));
        }

        if (!EmailRegex().IsMatch(value))
        {
            throw new ArgumentException($"Invalid email format: {value}", nameof(value));
        }

        Value = value;
    }

    public static implicit operator Email(string email) => new (email);

    public static implicit operator string(Email email) => email.Value;

    // Overriding ToString for meaningful display
    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
