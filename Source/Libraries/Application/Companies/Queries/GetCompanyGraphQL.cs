namespace Application.Companies;

public sealed record GetCompanyGraphQL : IRequest<IQueryable<Company>>
{
    public int StudentId { get; }

    public GetCompanyGraphQL(int studentId) => StudentId = studentId;
}

public class GetCompanyGraphQlHandler : IRequestHandler<GetCompanyGraphQL, IQueryable<Company>>
{
    private readonly IApplicationDbContext context;

    public GetCompanyGraphQlHandler(IApplicationDbContext context)
                    => this.context = context;

    public Task<IQueryable<Company>> Handle(GetCompanyGraphQL request, CancellationToken cancellationToken)
                => Task.FromResult(context.Companies.Where(c => c.StudentId == request.StudentId));

}
