using System.Linq;
using System.Threading.Tasks;
using Core.Contracts.Request;
using Core.Contracts.Response;
using Core.Entities;
using HotChocolate.Types.Relay;
using Infrastructure.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

using System;

namespace AlumniBackendServices.GraphQL
{
    public class StudentQuery
    {
        private readonly IMediator mediator;

        public StudentQuery(IMediator mediator)
            => this.mediator = mediator;

        [UsePaging]
        public async Task<IQueryable<Student>> GetAllStudentAsync()
        {
            Console.WriteLine("Student");
            var query = new GetAllStudentGraphQL();
            return await mediator.Send(query);
        }

        public async Task<StudentResponse> GetStudentAsync(string email)
        {
            var query = new GetStudentQuery(email);
            var response = await mediator.Send(query);
            return response.Result;
        }

    }
}
