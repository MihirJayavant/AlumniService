using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using HotChocolate;
using Infrastructure.Queries;
using MediatR;

namespace AlumniBackendServices.GraphQL
{
    public class CompanyQuery
    {
        private readonly IMediator mediator;

        public CompanyQuery(IMediator mediator)
                => this.mediator = mediator;

        // public async Task<IEnumerable<CompanyResponse>> GetComapanyAsync(Guid studentId)
        // {
        //     var query = new GetCompanyQuery(studentId);
        //     var response = await mediator.Send(query);
        //     return response.Result;
        // }

        public async Task<IEnumerable<Company>> GetComapanyAsync([Parent] Student student)
        {
            Console.WriteLine("Company");
            var query = new GetCompanyGraphQL(student.StudentId);
            var result = await mediator.Send(query);
            return result.ToList();
        }
    }
}
