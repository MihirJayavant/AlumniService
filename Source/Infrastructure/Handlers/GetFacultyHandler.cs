using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Infrastructure.Queries;
using Core.Contracts.Response;
using AutoMapper.QueryableExtensions;
using Database;

namespace Infrastructure.Handlers
{
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
                                    .FirstOrDefaultAsync(f => f.Email == request.FacultyEmail);

            return response;
        }

    }
}
