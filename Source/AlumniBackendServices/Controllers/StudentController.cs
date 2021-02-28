using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Contracts.Response;
using MediatR;
using Infrastructure.Queries;
using Infrastructure.Commands;
using Core.Contracts.Request;
using Microsoft.AspNetCore.Authorization;

namespace AlumniBackendServices.Controllers
{
    [Authorize]
    [ProducesErrorResponseType(typeof(ErrorResponse))]
    [ProducesResponseType(400)]
    public class StudentController : ApiController
    {
        public StudentController(IMediator mediator): base(mediator) {}

        // GET api/student
        [HttpGet]
        [Produces(typeof(AllStudentResponse))]
        public async Task<IActionResult> Get( [FromQuery] GetAllStudentRequest request)
        {
            var query = new GetAllStudentQuery(request.PageNumber, request.PageSize);
            var response = await mediator.Send(query);
            return GetResult(response);
        }

        //GET api/student/asddsa21324asa
        [HttpGet("{email}")]
        [Produces(typeof(StudentResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var query = new GetStudentQuery(email);
            var response = await mediator.Send(query);
            return GetResult(response);
        }

        // POST api/Student
        [HttpPost]
        [Produces(typeof(StudentResponse))]
        public async Task<IActionResult> PostAsync([FromBody]AddStudentCommand student)
        {
            var response = await mediator.Send(student);
            return GetResult(response);
        }

    }
}
