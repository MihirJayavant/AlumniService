using FluentValidation;

namespace Core;

#pragma warning disable CA1708

public static class ValidationExtensions
{
    extension<T>(IRuleBuilderInitial<T, string> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string> ValidEmail(int maximumLength = 254)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumLength);

            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(value => value.Trim().Length <= maximumLength)
                .WithMessage($"{{PropertyName}} must be at most {maximumLength} characters.")
                .Must(IsValidEmail)
                .WithMessage("{PropertyName} must be a valid email address.");
        }
    }

    extension<T>(IRuleBuilder<T, Guid> ruleBuilder)
    {
        public IRuleBuilderOptions<T, Guid> ValidGuid()
        => ruleBuilder.NotEmpty().WithMessage("{PropertyName} must be a non-empty GUID.");
    }

    extension<T>(AbstractValidator<T> validator) where T : PaginationInput
    {
        public void ApplyPaginationRules(int maximumPageSize = 100)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumPageSize);

            validator.RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageNumber must be at least 1.");

            validator.RuleFor(x => x.PageSize)
                .InclusiveBetween(1, maximumPageSize)
                .WithMessage($"PageSize must be between 1 and {maximumPageSize}.");

            validator.RuleFor(x => x.PageNumber)
                .Must((request, pageNumber) => pageNumber < 1 || request.PageSize < 1
                    || ((long)pageNumber - 1) * request.PageSize <= int.MaxValue)
                .WithMessage("The requested page offset is too large.");
        }
    }

    extension<T>(IRuleBuilderInitial<T, string> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string> RequiredText(int maximumLength)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumLength);

            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(value => value.Trim().Length <= maximumLength)
                .WithMessage($"{{PropertyName}} must be at most {maximumLength} characters.");
        }

        public IRuleBuilderOptions<T, string> ValidMobileNumber()
            => ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(value => value.Trim().Length <= 15
                    && value.Trim().All(char.IsAsciiDigit)
                    && value.Trim().Any(c => c != '0'))
                .WithMessage("{PropertyName} must contain between 1 and 15 ASCII digits and must not be all zeros.");

        public IRuleBuilderOptions<T, string> ValidGender()
            => ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(value => string.Equals(value.Trim(), "Male", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value.Trim(), "Female", StringComparison.OrdinalIgnoreCase))
                .WithMessage("{PropertyName} must be Male or Female.");
    }

    extension<T>(IRuleBuilderInitial<T, int> ruleBuilder)
    {
        public IRuleBuilderOptions<T, int> ValidYear(int minimumYear = 1900, TimeProvider? timeProvider = null)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(minimumYear, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(minimumYear, 9999);
            var clock = timeProvider ?? TimeProvider.System;

            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .Must(value => value >= minimumYear && value <= clock.GetUtcNow().Year)
                .WithMessage(_ => $"{{PropertyName}} must be between {minimumYear} and {clock.GetUtcNow().Year}.");
        }
    }

    extension<T>(IRuleBuilderInitial<T, DateOnly> ruleBuilder)
    {
        public IRuleBuilderOptions<T, DateOnly> ValidDateOfBirth(int minimumAge = 10, int maximumAge = 100, TimeProvider? timeProvider = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(minimumAge);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maximumAge, minimumAge);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(maximumAge, 9998);
            var clock = timeProvider ?? TimeProvider.System;

            return ruleBuilder.Must(value =>
            {
                var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
                if (value == DateOnly.MinValue || value > today || value.Year > 9999 - minimumAge)
                {
                    return false;
                }

                return value.AddYears(minimumAge) < today
                    && (value.Year > 9999 - maximumAge || value.AddYears(maximumAge) >= today);
            }).WithMessage($"{{PropertyName}} must represent an age greater than {minimumAge} and no older than {maximumAge} years.");
        }
    }

    private static bool IsValidEmail(string value)
    {
        try
        {
            _ = new Email(value);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
