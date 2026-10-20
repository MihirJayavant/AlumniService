namespace Alumni.Network.FurtherStudies;

public record GetFurtherStudy
{
    public Guid MemberId { get; init; }
}

file sealed class GetFurtherStudyValidator : AbstractValidator<GetFurtherStudy>
{
    public GetFurtherStudyValidator() => RuleFor(x => x.MemberId).ValidGuid();
}

public class GetFurtherStudyHandler(IMemberDbContext context)
    : IHandler<GetFurtherStudy, PaginatedList<FurtherStudyResponse>>
{
    public AbstractValidator<GetFurtherStudy> Validator { get; } = new GetFurtherStudyValidator();

    public async Task<OneOf<PaginatedList<FurtherStudyResponse>, ErrorType>> Handle(GetFurtherStudy request, CancellationToken cancellationToken)
    {
        var member = await context.Members.FirstOrDefaultAsync(s => s.MemberId == request.MemberId, cancellationToken);
        if (member is null)
        {
            return new ErrorType { Message = "Member not found", Status = ResponseStatus.NotFound };
        }

        var result = await context.FurtherStudies
            .Where(s => s.MemberId == member.Id)
            .Paginate(1, 50, cancellationToken);

        return result.WithItems(f => f.ToFurtherStudyResponse());
    }
}
