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
    public class FacultyController : ApiController
    {

        public FacultyController(IMediator mediator): base(mediator) {}


        #region Actions

        // GET: api/Facutly
        [Produces(typeof(IEnumerable<FacultyResponse>))]
        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            var query = new GetAllFacultiesQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [Produces(typeof(FacultyResponse))]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetAsync(string email)
        {
            var query = new GetFacultyQuery(email);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        // POST: api/Facutly
        [HttpPost()]
        public async Task<IActionResult> PostAsync([FromBody]AddFacultyCommand faculty)
        {
            await mediator.Send(faculty);
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{facultyId}")]
        public async Task<IActionResult> DeleteAsync(int facultyId)
        {
            var query = new DeleteFacultyCommand(facultyId);
            await mediator.Send(query);
            return Ok();
        }

        #endregion
    }
}
