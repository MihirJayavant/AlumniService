namespace Alumni.Staff;

public sealed record GetStaffMembers : PaginationInput
{

}

public sealed class GetStaffMembersValidator : AbstractValidator<GetStaffMembers>
{
    public GetStaffMembersValidator() => this.ApplyPaginationRules();
}

public class GetStaffMembersHandler(IStaffDbContext context)
    : IHandler<GetStaffMembers, PaginatedList<StaffMemberResponse>>
{
    public AbstractValidator<GetStaffMembers> Validator { get; } = new GetStaffMembersValidator();

    public async Task<OneOf<PaginatedList<StaffMemberResponse>, ErrorType>> Handle(GetStaffMembers request,
        CancellationToken cancellationToken)
    {
        var result = await context.StaffMembers
            .OrderBy(f => f.Id)
            .Paginate(request.PageNumber, request.PageSize, cancellationToken);
        return result.WithItems(f => f.ToStaffMemberResponse());
    }
}
