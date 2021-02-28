#pragma warning disable IDE1006

using AutoMapper;
using Database;
using Infrastructure.Services;

namespace Infrastructure.Handlers
{
    public abstract class BaseHandler
    {
        protected readonly ApplicationContext context;
        protected readonly IMapper mapper;
        protected readonly IValidationService validationService;


        public BaseHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        => (this.context, this.mapper, this.validationService) = (context, mapper, validationService);
    }
}
