using MediatR;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using Infrastructure.Commands;
using Database;

namespace Infrastructure.Handlers
{
    public class DeleteFacultyHandler : IRequestHandler<DeleteFacultyCommand>
    {
        private readonly ApplicationContext context;

        public DeleteFacultyHandler(ApplicationContext context)
                        => this.context = context;

        public async Task<Unit> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
        {
            var faculty = await context.Faculties
                            .FindAsync(request.FacultyId);

            if(faculty != null) {
                context.Faculties.Remove(faculty);
                await context.SaveChangesAsync();
            }
            return new Unit();
        }

    }
}
