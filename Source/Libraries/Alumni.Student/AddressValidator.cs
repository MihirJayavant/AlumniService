namespace Alumni.Student;

internal sealed class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator(int locationMaximumLength)
    {
        RuleFor(x => x.PinCode).NotEmpty();
        RuleFor(x => x.Country).RequiredText(locationMaximumLength);
        RuleFor(x => x.State).RequiredText(locationMaximumLength);
        RuleFor(x => x.City).RequiredText(locationMaximumLength);
        RuleFor(x => x.UserAddress).RequiredText(100);
    }
}
