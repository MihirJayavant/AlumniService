using AutoMapper.QueryableExtensions;

namespace Infrastructure.Handlers;

public class GetFacultyHandler : IRequestHandler<GetFacultyQuery, FacultyResponse>
{
    private readonly ApplicationContext context;
    private readonly IMapper mapper;
    public GetFacultyHandler(ApplicationContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<FacultyResponse> Handle(GetFacultyQuery request, CancellationToken cancellationToken)
    {
        var response = await context.Faculties
                                .ProjectTo<FacultyResponse>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(f => f.Email == request.FacultyEmail, cancellationToken);

        return response!;
    }

}
