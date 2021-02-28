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
    public class GetFurtherStudyHandler : BaseHandler,
                    IRequestHandler<GetFurtherStudyQuery,Response<IEnumerable<FurtherStudyResponse>>>
    {

        public GetFurtherStudyHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        : base(context, mapper, validationService) {}

        public async Task<Response<IEnumerable<FurtherStudyResponse>>> Handle(GetFurtherStudyQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FurtherStudyResponse> result =
                                await context.FurtherStudies
                                        .Where(s => s.StudentId == request.StudentId)
                                        .ProjectTo<FurtherStudyResponse>(mapper.ConfigurationProvider)
                                        .ToListAsync(cancellationToken);

            return validationService.CreateSuccessResponse(result);
        }

    }
}
