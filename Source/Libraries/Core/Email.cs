using System.Text.RegularExpressions;

namespace Core;

public readonly partial record struct Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));
        }

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (normalizedValue.Length > 254
            || normalizedValue.IndexOf('@') > 64
            || !EmailRegex().IsMatch(normalizedValue))
        {
            throw new ArgumentException($"Invalid email format: {value}", nameof(value));
        }

        Value = normalizedValue;
    }

    public static implicit operator Email(string email) => new(email);

    public static implicit operator string(Email email) => email.Value;

    // Overriding ToString for meaningful display
    public override string ToString() => Value;

    // Unquoted ASCII dot-atom local part and a dotted DNS domain.
    // Domain labels contain 1-63 characters and cannot start or end with a hyphen.
    [GeneratedRegex(@"\A[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)+\z")]
    private static partial Regex EmailRegex();
}
