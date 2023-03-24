using AutoMapper.QueryableExtensions;

namespace Application.Common.Handlers;

public class GetAllFacultiesHandler : IRequestHandler<GetAllFacultiesQuery, IEnumerable<FacultyResponse>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public GetAllFacultiesHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<IEnumerable<FacultyResponse>> Handle(GetAllFacultiesQuery request, CancellationToken cancellationToken)
    {
        var response = await context.Faculties
                                .ProjectTo<FacultyResponse>(mapper.ConfigurationProvider)
                                .ToListAsync(cancellationToken);

        return response;
    }

}
