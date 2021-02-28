// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using AlumniServices.Library.MySql.Models;
// using AlumniServices.Library.MySql.Context;
// using AlumniServices.Library.MySql.Models.Views;
// using Microsoft.AspNetCore.Authorization;

// namespace AlumniBackendServices.Controllers
// {
//     [Produces("application/json")]
//     [Route("api/[controller]")]
//     //[Authorize]
//     public class StudentQueryController : ControllerBase
//     {
//         readonly StudentQueryContext context;

//         public StudentQueryController(StudentQueryContext context)
//         {
//             this.context = context;
//         }
        
//         // GET: api/StudentQuery/5
//         [HttpGet()]
//         public async Task<IActionResult> GetStudentAsync(StudentQuery query)
//         {
//             IEnumerable<StudentView> result = await context.GetStudents(query);

//             return Ok(result);
//         }

//         [HttpGet("alumni")]
//         public async Task<IActionResult> GetAlumniAsync(CompanyQuery query)
//         {
//             IEnumerable<StudentView> result = await context.GetAlumnis(query);

//             return Ok(result);
//         }

//     }
// }
