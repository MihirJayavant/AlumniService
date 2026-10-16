namespace Alumni.Staff;

public sealed record GetStaffMember
{
    public Guid FacultyId { get; init; }
}

file sealed class GetStaffMemberValidator : AbstractValidator<GetStaffMember>
{
    public GetStaffMemberValidator() => RuleFor(x => x.FacultyId).ValidGuid();
}

public class GetStaffMemberHandler(IStaffDbContext context) : IHandler<GetStaffMember, StaffMemberResponse>
{
    public AbstractValidator<GetStaffMember> Validator { get; } = new GetStaffMemberValidator();

    public async Task<OneOf<StaffMemberResponse, ErrorType>> Handle(GetStaffMember request, CancellationToken cancellationToken)
    {
        var result = await context.StaffMembers
                                .Where(f => f.FacultyId == request.FacultyId)
                                .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            return new ErrorType { Status = ResponseStatus.NotFound, Message = "Faculty not found" };
        }
        return result.ToStaffMemberResponse();
    }
}
