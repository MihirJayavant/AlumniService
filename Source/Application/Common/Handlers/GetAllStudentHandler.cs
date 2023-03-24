using AutoMapper.QueryableExtensions;

namespace Application.Common.Handlers;

public class GetAllStudentHandler
            : BaseHandler, IRequestHandler<GetAllStudentQuery, Response<AllStudentResponse>>
{

    public GetAllStudentHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService) { }

    public async Task<Response<AllStudentResponse>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
    {
        var result = await context.Students
                                    .Skip(request.Pagination.PageNumber - 1)
                                    .Take(request.Pagination.PageSize)
                                    .ProjectTo<StudentResponse>(mapper.ConfigurationProvider)
                                    .ToListAsync(cancellationToken);

        var count = await context.Students.CountAsync(cancellationToken);
        var totalPages = (count + 0.0) / request.Pagination.PageSize;

        var response = new AllStudentResponse
        {
            Data = result,
            PageNumber = request.Pagination.PageNumber,
            PageSize = request.Pagination.PageSize,
            TotalItems = count,
            TotalPages = (int)Math.Ceiling(totalPages)
        };
        return validationService.CreateSuccessResponse(response);
    }
}
