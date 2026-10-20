namespace Alumni.Network;

[RecordView(typeof(Member))]
public sealed partial record AddMember
{

}

file sealed class AddMemberValidator : AbstractValidator<AddMember>
{
    public AddMemberValidator()
    {
        RuleFor(x => x.Email).ValidEmail(maximumLength: 100);
        RuleFor(x => x.FirstName).RequiredText(100);
        RuleFor(x => x.LastName).RequiredText(100);
        RuleFor(x => x.Extension).RequiredText(10);
        RuleFor(x => x.MobileNo).ValidMobileNumber();
        RuleFor(x => x.Gender).ValidGender();
        RuleFor(x => x.Branch).RequiredText(30);
        RuleFor(x => x.DateOfBirth).ValidDateOfBirth();
        RuleFor(x => x.AdmissionYear)
            .ValidYear()
            .GreaterThanOrEqualTo(x => x.DateOfBirth.Year)
            .WithMessage("AdmissionYear must not precede the birth year.");
        RuleFor(x => x.PassingYear)
            .ValidYear()
            .GreaterThanOrEqualTo(x => x.AdmissionYear)
            .WithMessage("PassingYear must not precede AdmissionYear.");
        RuleFor(x => x.CurrentAddress).NotNull().SetValidator(new AddressValidator(100));
        RuleFor(x => x.CorrespondenceAddress).NotNull().SetValidator(new AddressValidator(100));
    }
}


public class AddMemberHandler(IMemberDbContext context) : IHandler<AddMember, MemberResponse>
{
    public AbstractValidator<AddMember> Validator { get; } = new AddMemberValidator();

    public async Task<OneOf<MemberResponse, ErrorType>> Handle(AddMember request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email).Value;
        var account = await context.Members
                    .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);

        if (account is not null)
        {
            return new ErrorType
            {
                Message = "Member with this email already exists",
                Status = ResponseStatus.Conflict
            };
        }

        var member = request.ToMember();
        context.Members.Add(member);
        await context.SaveChangesAsync(cancellationToken);

        return member.ToMemberResponse();
    }
}


public static class AddMemberMapper
{
    public static MemberEntity ToMember(this AddMember member) =>
        new()
        {
            Id = 0,
            MemberId = Guid.CreateVersion7(),
            FirstName = member.FirstName.Trim(),
            LastName = member.LastName.Trim(),
            MobileNo = member.MobileNo.Trim(),
            Extension = member.Extension.Trim(),
            Gender = string.Equals(member.Gender.Trim(), Gender.Male, StringComparison.OrdinalIgnoreCase)
                ? Gender.Male : Gender.Female,
            DateOfBirth = member.DateOfBirth,
            Email = new Email(member.Email).Value,
            Branch = member.Branch.Trim(),
            CurrentAddress = NormalizeAddress(member.CurrentAddress),
            CorrespondenceAddress = NormalizeAddress(member.CorrespondenceAddress),
            AdmissionYear = member.AdmissionYear,
            PassingYear = member.PassingYear,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

    private static Address NormalizeAddress(Address address) => address with
    {
        PinCode = address.PinCode.Trim(),
        Country = address.Country.Trim(),
        State = address.State.Trim(),
        City = address.City.Trim(),
        UserAddress = address.UserAddress.Trim()
    };
}
