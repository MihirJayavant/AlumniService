using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Queries;
using AlumniBackendServices.Services;

namespace AlumniBackendServices.Controllers
{
    public class IdentityController : ApiController
    {

        private readonly IIdentityService identity;

        public IdentityController(IMediator mediator, IIdentityService identity) : base(mediator)
            => this.identity = identity;

        [HttpPost("student/registration")]
        public async Task<IActionResult> StudentRegistration([FromBody] StudentRegistration body)
        {
            var result = await identity.RegisterStudent(body.Email, body.Password);
            return GetResult(result);
        }

        [HttpPost("student/login")]
        public async Task<IActionResult> StudentLogin([FromBody] StudentRegistration body)
        {
            var result = await identity.StudentLogin(body.Email, body.Password);
            return GetResult(result);
        }
    }
}
