using MediatR;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using Infrastructure.Queries;
using Database;
using System.Linq;
using Core.Entities;

namespace Infrastructure.Handlers
{
    public class GetCompanyGraphQlHandler : IRequestHandler<GetCompanyGraphQL, IQueryable<Company>>
    {
        private readonly ApplicationContext context;

        public GetCompanyGraphQlHandler(ApplicationContext context)
                        => this.context = context;

        public async Task<IQueryable<Company>> Handle(GetCompanyGraphQL request, CancellationToken cancellationToken)
                    => await Task.FromResult(context.Companies.Where( c => c.StudentId == request.StudentId));

    }
}

