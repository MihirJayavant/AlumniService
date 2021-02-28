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
    public class FurtherStudiesController : ApiController
    {

        public FurtherStudiesController(IMediator mediator): base(mediator) {}

        #region Actions

        // GET: api/FurtherStudies/asdad253adss
        [HttpGet("{studentId}")]
        [Produces(typeof(IEnumerable<FurtherStudyResponse>))]
        public async Task<IActionResult> GetAsync(int studentId)
        {
            var query = new GetFurtherStudyQuery(studentId);
            var result = await mediator.Send(query);
            return GetResult(result);
        }

        // POST: api/FurtherStudies
        [HttpPost]
        [Produces(typeof(FurtherStudyResponse))]
        [ProducesResponseType(404), ProducesResponseType(400)]
        public async Task<IActionResult> PostAsync([FromBody]AddFurtherStudyCommand study)
        {
            var result = await mediator.Send(study);
            return GetResult(result);
        }

        #endregion
    }
}
