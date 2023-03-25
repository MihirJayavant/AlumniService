using Application.Common.Models;

namespace Application.Common.Handlers;

public class AddFurtherStudyHandler : BaseHandler,
                    IRequestHandler<AddFurtherStudyCommand, Response<FurtherStudyResponse>>
{
    public AddFurtherStudyHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService) { }

    public async Task<Response<FurtherStudyResponse>> Handle(AddFurtherStudyCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students
                        .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (student is null)
        {
            return validationService
                        .CreateErrorResponse<FurtherStudyResponse>("Student not found", ResponseStatus.NotFound);
        }

        var furtherStudy = mapper.Map<FurtherStudy>(request);
        await context.FurtherStudies.AddAsync(furtherStudy, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        var response = mapper.Map<FurtherStudyResponse>(furtherStudy);

        return validationService.CreateSuccessResponse(response);
    }

}
