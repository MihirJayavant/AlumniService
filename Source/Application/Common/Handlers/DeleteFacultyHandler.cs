namespace Application.Common.Handlers;

public class DeleteFacultyHandler : IRequestHandler<DeleteFacultyCommand>
{
    private readonly IApplicationDbContext context;

    public DeleteFacultyHandler(IApplicationDbContext context)
                    => this.context = context;

    public async Task Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await context.Faculties
                        .FindAsync(new object[] { request.FacultyId }, cancellationToken: cancellationToken);

        if (faculty is not null)
        {
            context.Faculties.Remove(faculty);
            await context.SaveChangesAsync(cancellationToken);
        }
        return;
    }

}
