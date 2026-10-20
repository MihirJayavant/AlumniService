namespace Alumni.Network;

public sealed record GetMembers : PaginationInput
{

}

public sealed class GetMembersValidator : AbstractValidator<GetMembers>
{
    public GetMembersValidator() => this.ApplyPaginationRules();
}

public sealed class GetMembersHandler(IMemberDbContext context)
    : IHandler<GetMembers, PaginatedList<MemberResponse>>
{
    public AbstractValidator<GetMembers> Validator { get; } = new GetMembersValidator();

    public async Task<OneOf<PaginatedList<MemberResponse>, ErrorType>> Handle(GetMembers request,
        CancellationToken cancellationToken = default)
    {
        var result = await context.Members
            .OrderBy(s => s.Id)
            .Paginate(request.PageNumber, request.PageSize, cancellationToken);

        return result.WithItems(s => s.ToMemberResponse());
    }
}
