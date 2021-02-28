using MediatR;
using Core.Contracts.Response;
using Infrastructure.Queries;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Database;
using System;
using Infrastructure.Services;

namespace Infrastructure.Handlers
{
    public class GetAllStudentHandler
                : BaseHandler, IRequestHandler<GetAllStudentQuery,Response<AllStudentResponse>>
    {

        public GetAllStudentHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        : base(context, mapper, validationService) {}

        public async Task<Response<AllStudentResponse>> Handle(GetAllStudentQuery request, CancellationToken cancellationToken)
        {
            var result = await context.Students
                                        .Skip(request.Pagination.PageNumber - 1)
                                        .Take(request.Pagination.PageSize)
                                        .ProjectTo<StudentResponse>(mapper.ConfigurationProvider)
                                        .ToListAsync();

            var count = await context.Students.CountAsync();
            var totalPages = (count + 0.0) / request.Pagination.PageSize;

            var response = new AllStudentResponse
                            {
                                Data = result,
                                PageNumber = request.Pagination.PageNumber,
                                PageSize = request.Pagination.PageSize,
                                TotalItems = count,
                                TotalPages = (int) Math.Ceiling(totalPages)
                            };
            return validationService.CreateSuccessResponse(response);
        }
    }
}
