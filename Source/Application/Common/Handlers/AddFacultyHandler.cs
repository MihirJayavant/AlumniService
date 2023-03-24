namespace Application.Common.Handlers;

public class AddFacultyHandler : BaseHandler, IRequestHandler<AddFacultyCommand>
{

    public AddFacultyHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService) { }


    public async Task Handle(AddFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await context.Faculties
                        .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (faculty == null)
        {
            var f = mapper.Map<Faculty>(request);
            await context.Faculties.AddAsync(f, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        return;
    }

}
