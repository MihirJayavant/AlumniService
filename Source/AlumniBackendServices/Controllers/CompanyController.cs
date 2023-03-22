using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Infrastructure.Queries;
using Core.Contracts.Response;
using Infrastructure.Commands;
using Microsoft.AspNetCore.Authorization;

namespace AlumniBackendServices.Controllers
{
    [Authorize]
    public class CompanyController : ApiController
    {

        public CompanyController(IMediator mediator): base(mediator) {}

        #region Actions

        // GET api/<controller>/5
        [HttpGet("{studentId}")]
        [Produces(typeof(IEnumerable<CompanyResponse>))]
        public async Task<IActionResult> GetAsyncByID(int studentId)
        {
            var query = new GetCompanyQuery(studentId);
            var response = await mediator.Send(query);
            return GetResult(response);
        }

        // POST api/<controller>/company
        [HttpPost]
        [Produces(typeof(CompanyResponse))]
        [ProducesResponseType(404), ProducesResponseType(400)]
        public async Task<IActionResult> PostAsync([FromBody]AddCompanyCommand company)
        {
            var response = await mediator.Send(company);
            return GetResult(response);
        }

        #endregion
    }
}
