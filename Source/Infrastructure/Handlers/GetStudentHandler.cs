using AutoMapper.QueryableExtensions;

namespace Infrastructure.Handlers;

public class GetStudentHandler : BaseHandler, IRequestHandler<GetStudentQuery,Response<StudentResponse>>
{

    public GetStudentHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService){}

    public async Task<Response<StudentResponse>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var result = await context.Students
                                    .Where(s => s.Email == request.Email)
                                    .ProjectTo<StudentResponse>(mapper.ConfigurationProvider)
                                    .FirstOrDefaultAsync();
        if(result is null)
        {
            return validationService
                        .CreateErrorResponse<StudentResponse>("Student not found", ResponseStatus.NotFound);
        }

        return validationService.CreateSuccessResponse(result);
    }

}
