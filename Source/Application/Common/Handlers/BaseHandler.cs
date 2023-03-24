#pragma warning disable IDE1006

namespace Application.Common.Handlers;

public abstract class BaseHandler
{
    protected readonly IApplicationDbContext context;
    protected readonly IMapper mapper;
    protected readonly IValidationService validationService;


    public BaseHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    => (this.context, this.mapper, this.validationService) = (context, mapper, validationService);
}
