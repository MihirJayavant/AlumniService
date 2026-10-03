using FluentValidation;

namespace Core;

public static class ValidationExtensions
{
    extension<T>(IRuleBuilderInitial<T, string> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string> ValidEmail(
int maximumLength = 254)
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
