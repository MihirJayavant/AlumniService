namespace Application.Common.Handlers;

public class GetAllStudentGraphQlHandler : IRequestHandler<GetAllStudentGraphQL, IQueryable<Student>>
{
    private readonly IApplicationDbContext context;

    public GetAllStudentGraphQlHandler(IApplicationDbContext context)
                    => this.context = context;

    public async Task<IQueryable<Student>> Handle(GetAllStudentGraphQL request, CancellationToken cancellationToken)
                => await Task.Run(() => context.Students);

}

