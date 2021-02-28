using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlumniBackendServices.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationContext context) : base(context) {}

        public async Task<IEnumerable<Student>> GetProfile() =>
                    await context.Students.Take(100)
                                            .ToListAsync();

    }
}
