namespace Alumni.Staff;

public sealed record DeleteStaffMember
{
    public Guid FacultyId { get; init; }
}

file sealed class DeleteStaffMemberValidator : AbstractValidator<DeleteStaffMember>
{
    public DeleteStaffMemberValidator() => RuleFor(x => x.FacultyId).ValidGuid();
}

public class DeleteStaffMemberHandler(IStaffDbContext context)
    : IHandler<DeleteStaffMember, StaffMemberResponse>
{
    public AbstractValidator<DeleteStaffMember> Validator { get; } = new DeleteStaffMemberValidator();

    public async Task<OneOf<StaffMemberResponse, ErrorType>> Handle(DeleteStaffMember request, CancellationToken cancellationToken)
    {
        var staffMember = await context.StaffMembers.FirstOrDefaultAsync(f => f.FacultyId == request.FacultyId, cancellationToken);

        if (staffMember is null)
        {
            return new ErrorType { Status = ResponseStatus.NotFound, Message = "Faculty not found" };
        }

        context.StaffMembers.Remove(staffMember);
        await context.SaveChangesAsync(cancellationToken);
        return staffMember.ToStaffMemberResponse();
    }
}
