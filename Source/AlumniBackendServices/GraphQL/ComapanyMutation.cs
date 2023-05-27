// using System.Threading.Tasks;
// using Application.Companies;
// using Application.Contracts.Response;
// using MediatR;

// namespace AlumniBackendServices.GraphQL
// {
//     public class CompanyMutation
//     {
//         private readonly IMediator mediator;

//         public CompanyMutation(IMediator mediator) => this.mediator = mediator;

//         public async Task<CompanyResponse> AddCompanyAsync(AddCompanyCommand company)
//         {
//             var response = await mediator.Send(company);
//             return null;
//         }
//     }
// }
