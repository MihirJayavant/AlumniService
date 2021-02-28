using MediatR;
using Core.Contracts.Response;
using System.Collections.Generic;

namespace Infrastructure.Queries
{
    public class GetAllFacultiesQuery : IRequest<IEnumerable<FacultyResponse>>
    {

    }
}
