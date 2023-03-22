using FluentValidation;

namespace Infrastructure.Validators;

public class AddCompanyValidator : AbstractValidator<AddCompanyCommand>
{
    public AddCompanyValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty();
        RuleFor(c => c.Designation).NotEmpty();
    }
}
