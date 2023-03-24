using AutoMapper.QueryableExtensions;

namespace Application.Common.Handlers;

public class GetExamHandler
: BaseHandler, IRequestHandler<GetExamQuery, Response<IEnumerable<ExamResponse>>>
{

    public GetExamHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService) { }

    public async Task<Response<IEnumerable<ExamResponse>>> Handle(GetExamQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ExamResponse> result =
                            await context.Exams
                                    .Where(s => s.StudentId == request.StudentId)
                                    .ProjectTo<ExamResponse>(mapper.ConfigurationProvider)
                                    .ToListAsync(cancellationToken);

        return validationService.CreateSuccessResponse(result);
    }

}
