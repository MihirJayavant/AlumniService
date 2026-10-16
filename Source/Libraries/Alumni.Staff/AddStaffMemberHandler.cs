namespace Alumni.Staff;

[RecordView(typeof(StaffMember), nameof(StaffMember.FacultyId))]
public sealed partial record AddStaffMember
{

}

file sealed class AddStaffMemberValidator : AbstractValidator<AddStaffMember>
{
    public AddStaffMemberValidator()
    {
        RuleFor(x => x.Email).ValidEmail(maximumLength: 100);

        RuleFor(x => x.FirstName).RequiredText(100).WithMessage("FirstName must be at most 100 characters.");
        RuleFor(x => x.LastName).RequiredText(100).WithMessage("LastName must be at most 100 characters.");
        RuleFor(x => x.Extension).RequiredText(10);

        RuleFor(x => x.MobileNo).GreaterThan(0);
    }
}

public class AddStaffMemberHandler(IStaffDbContext context) : IHandler<AddStaffMember, StaffMemberResponse>
{
    public AbstractValidator<AddStaffMember> Validator { get; } = new AddStaffMemberValidator();

    public async Task<OneOf<StaffMemberResponse, ErrorType>> Handle(AddStaffMember request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email).Value;
        var found = await context.StaffMembers
                    .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);

        if (found is not null)
        {
            return new ErrorType
            {
                Message = "Faculty with this email already exists",
                Status = ResponseStatus.Conflict
            };
        }

        var createdAt = DateTime.UtcNow;
        var staffMember = new StaffMemberEntity()
        {
            Id = 0,
            FacultyId = Guid.NewGuid(),
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Extension = request.Extension.Trim(),
            MobileNo = request.MobileNo,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
        context.StaffMembers.Add(staffMember);

        await context.SaveChangesAsync(cancellationToken);

        return staffMember.ToStaffMemberResponse();
    }
}
