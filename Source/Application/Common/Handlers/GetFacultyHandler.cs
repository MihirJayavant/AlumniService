using AutoMapper.QueryableExtensions;

namespace Application.Common.Handlers;

public class GetFacultyHandler : IRequestHandler<GetFacultyQuery, FacultyResponse>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public GetFacultyHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public async Task<FacultyResponse> Handle(GetFacultyQuery request, CancellationToken cancellationToken)
    {
        var response = await context.Faculties
                                .ProjectTo<FacultyResponse>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(f => f.Email == request.FacultyEmail, cancellationToken);

        return response!;
    }

}
