namespace Application.Companies;

public sealed record GetCompanyGraphQL : IRequest<IQueryable<Core.Entities.Company>>
{
    public int StudentId { get; }

    public GetCompanyGraphQL(int studentId) => StudentId = studentId;
}

public class GetCompanyGraphQlHandler : IRequestHandler<GetCompanyGraphQL, IQueryable<Core.Entities.Company>>
{
    private readonly IApplicationDbContext context;

    public GetCompanyGraphQlHandler(IApplicationDbContext context)
                    => this.context = context;

    public Task<IQueryable<Core.Entities.Company>> Handle(GetCompanyGraphQL request, CancellationToken cancellationToken)
                => Task.FromResult(context.Companies.Where(c => c.StudentId == request.StudentId));

}
