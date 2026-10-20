namespace Alumni.Network;

public sealed record GetMember
{
    public Guid Id { get; init; }
}

file sealed class GetMemberValidator : AbstractValidator<GetMember>
{
    public GetMemberValidator() => RuleFor(x => x.Id).ValidGuid();
}

public class GetMemberHandler(IMemberDbContext context) : IHandler<GetMember, MemberResponse>
{
    public AbstractValidator<GetMember> Validator { get; } = new GetMemberValidator();

    public async Task<OneOf<MemberResponse, ErrorType>> Handle(GetMember request,
        CancellationToken cancellationToken)
    {
        var result = await context.Members
            .Where(s => s.MemberId == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return result switch
        {
            null => new ErrorType { Status = ResponseStatus.NotFound, Message = "Members not found" },
            _ => result.ToMemberResponse()
        };
    }
}
