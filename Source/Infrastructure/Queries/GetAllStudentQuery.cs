using MediatR;
using Core.Contracts.Response;
using System.Collections.Generic;

namespace Infrastructure.Queries
{
    public class GetAllStudentQuery : IRequest<Response<AllStudentResponse>>
    {
        public PaginationQuery Pagination { get; }

        public GetAllStudentQuery(int pageNumber, int pageSize)
                => Pagination = new PaginationQuery
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };

    }
}
