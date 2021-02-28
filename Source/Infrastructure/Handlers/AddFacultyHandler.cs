using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Core.Entities;
using Infrastructure.Commands;
using Database;
using Infrastructure.Services;

namespace Infrastructure.Handlers
{
    public class AddFacultyHandler : BaseHandler, IRequestHandler<AddFacultyCommand>
    {

        public AddFacultyHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        :base(context, mapper, validationService) {}


        public async Task<Unit> Handle(AddFacultyCommand request, CancellationToken cancellationToken)
        {
            var faculty = await context.Faculties
                            .FirstOrDefaultAsync(s => s.Email == request.Email);

            if(faculty == null)
            {
                var f = mapper.Map<Faculty>(request);
                await context.Faculties.AddAsync(f);
                await context.SaveChangesAsync();
            }
            return new Unit();
        }

    }
}
