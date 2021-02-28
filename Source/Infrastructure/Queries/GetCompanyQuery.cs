using MediatR;
using Core.Contracts.Response;
using System.Collections.Generic;
using System;

namespace Infrastructure.Queries
{
    public class GetCompanyQuery : IRequest<Response<IEnumerable<CompanyResponse>>>
    {
        public int StudentId { get; }

        public GetCompanyQuery(int studentId) => StudentId = studentId;
    }
}
