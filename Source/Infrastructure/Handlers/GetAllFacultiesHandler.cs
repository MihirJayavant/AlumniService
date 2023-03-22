using AutoMapper.QueryableExtensions;

namespace Infrastructure.Handlers;

public class GetAllFacultiesHandler : IRequestHandler<GetAllFacultiesQuery, IEnumerable<FacultyResponse>>
{
    private readonly ApplicationContext context;
    private readonly IMapper mapper;
    public GetAllFacultiesHandler(ApplicationContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<IEnumerable<FacultyResponse>> Handle(GetAllFacultiesQuery request, CancellationToken cancellationToken)
    {
        var response = await context.Faculties
                                .ProjectTo<FacultyResponse>(mapper.ConfigurationProvider)
                                .ToListAsync(cancellationToken);

        return response;
    }

}
