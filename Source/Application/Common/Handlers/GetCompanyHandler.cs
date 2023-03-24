using AutoMapper.QueryableExtensions;

namespace Application.Common.Handlers;

public class GetCompanyHandler : BaseHandler, IRequestHandler<GetCompanyQuery, Response<IEnumerable<CompanyResponse>>>
{
    public GetCompanyHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService) { }

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
