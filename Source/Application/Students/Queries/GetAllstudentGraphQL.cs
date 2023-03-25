namespace Application.Students;

public class GetAllStudentGraphQL : IRequest<IQueryable<Student>>
{

}

public class GetAllStudentGraphQlHandler : IRequestHandler<GetAllStudentGraphQL, IQueryable<Student>>
{
    private readonly IApplicationDbContext context;

    public GetAllStudentGraphQlHandler(IApplicationDbContext context)
                    => this.context = context;

    public Task<IQueryable<Student>> Handle(GetAllStudentGraphQL request, CancellationToken cancellationToken)
                => Task.FromResult(context.Students.AsQueryable());

}
