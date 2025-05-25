// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Application.Companies;
// using Core.Entities;
// using HotChocolate;
// using MediatR;
//
// namespace AlumniBackendServices.GraphQL
// {
//     public class CompanyQuery
//     {
//         private readonly IMediator mediator;
//
//         public CompanyQuery(IMediator mediator)
//                 => this.mediator = mediator;
//
//         public async Task<IEnumerable<Company>> GetCompanyAsync([Parent] Student student)
//         {
//             Console.WriteLine("Company");
//             var query = new GetCompanyGraphQL(student.StudentId);
//             var result = await mediator.Send(query);
//             return result.ToList();
//         }
//     }
// }
