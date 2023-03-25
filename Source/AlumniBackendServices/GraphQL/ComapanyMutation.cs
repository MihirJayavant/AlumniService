using System.Threading.Tasks;
using Application.Common.Commands;
using Application.Companies.Commands;
using Application.Contracts.Response;
using MediatR;

namespace AlumniBackendServices.GraphQL
{
    public class CompanyMutation
    {
        private readonly IMediator mediator;

        public CompanyMutation(IMediator mediator) => this.mediator = mediator;

        public async Task<CompanyResponse> AddComapanyAsync(AddCompanyCommand company)
        {
            var response = await mediator.Send(company);
            return response.Result;
        }
    }
}
