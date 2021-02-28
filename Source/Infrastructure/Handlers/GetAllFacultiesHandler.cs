using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Infrastructure.Queries;
using System.Collections.Generic;
using Core.Contracts.Response;
using AutoMapper.QueryableExtensions;
using Database;

namespace Infrastructure.Handlers
{
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
                                    .ToListAsync();

            return response;
        }

    }
}
