using MediatR;
using Core.Contracts.Response;
using System.Collections.Generic;

namespace Infrastructure.Queries
{
    public class PaginationQuery
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
