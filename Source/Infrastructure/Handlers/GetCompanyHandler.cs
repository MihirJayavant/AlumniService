using MediatR;
using Core.Contracts.Response;
using Infrastructure.Queries;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using Database;
using Infrastructure.Services;

namespace Infrastructure.Handlers
{
    public class GetCompanyHandler : BaseHandler, IRequestHandler<GetCompanyQuery,Response<IEnumerable<CompanyResponse>>>
    {

        public GetCompanyHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        : base(context, mapper, validationService) {}

        public async Task<Response<IEnumerable<CompanyResponse>>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<CompanyResponse> result =
                                await context.Companies
                                        .Where(s => s.StudentId == request.StudentId)
                                        .ProjectTo<CompanyResponse>(mapper.ConfigurationProvider)
                                        .ToListAsync(cancellationToken);

            return validationService.CreateSuccessResponse(result);
        }

    }
}
