using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace AlumniBackendServices.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<IEnumerable<Student>> GetProfile();
    }
}
