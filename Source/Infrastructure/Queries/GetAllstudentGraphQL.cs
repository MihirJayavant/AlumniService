using MediatR;
using System.Linq;
using Core.Entities;

namespace Infrastructure.Queries
{
    public class GetAllStudentGraphQL : IRequest<IQueryable<Student>>
    {

    }
}
