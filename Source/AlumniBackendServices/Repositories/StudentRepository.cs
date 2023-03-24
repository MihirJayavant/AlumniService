using Core.Entities;
using Database;
using Microsoft.EntityFrameworkCore;

namespace AlumniBackendServices.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(IApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Student>> GetProfile() =>
                await context.Students.Take(100)
                                        .ToListAsync();

}
