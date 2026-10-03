namespace Alumni.Student;

internal static class StudentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> RequiredText<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder, int maximumLength)
        => ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(value => value.Trim().Length <= maximumLength)
            .WithMessage($"{{PropertyName}} must be at most {maximumLength} characters.");
}
