using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using Infrastructure.Queries;
using Infrastructure.Commands;
using Core.Contracts.Response;
using Microsoft.AspNetCore.Authorization;

namespace AlumniBackendServices.Controllers
{
    [Authorize]
    public class ExamController : ApiController
    {

        public ExamController(IMediator mediatR): base(mediatR) { }


        #region Actions

        // GET: api/Exam/aassd2121sadd
        [HttpGet("{studentId}")]
        [Produces(typeof(IEnumerable<ExamResponse>))]
        public async Task<IActionResult> GetAsync(int studentId)
        {
            var query = new GetExamQuery(studentId);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        // POST: api/Exam
        [HttpPost()]
        [Produces(typeof(ExamResponse))]
        [ProducesResponseType(400), ProducesResponseType(404)]
        public async Task<IActionResult> PostAsync([FromBody]AddExamCommand exam)
        {
            var response = await mediator.Send(exam);
            return GetResult(response);
        }

        #endregion
    }
}
