namespace Infrastructure.Handlers;

public class GetAllStudentGraphQlHandler : IRequestHandler<GetAllStudentGraphQL, IQueryable<Student>>
{
    private readonly ApplicationContext context;

    public GetAllStudentGraphQlHandler(ApplicationContext context)
                    => this.context = context;

    public async Task<IQueryable<Student>> Handle(GetAllStudentGraphQL request, CancellationToken cancellationToken)
                => await Task.Run(() => context.Students);

}

