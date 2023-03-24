namespace Application.Common.Handlers;

public class AddStudentHandler : BaseHandler, IRequestHandler<AddStudentCommand, Response<StudentResponse>>
{
    public AddStudentHandler(IApplicationDbContext context, IMapper mapper, IValidationService validationService)
                    : base(context, mapper, validationService) { }

    public async Task<Response<StudentResponse>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
    {
        var account = await context.Students
                        .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (account is null)
        {
            var student = mapper.Map<Student>(request);
            student.DateCreated = DateTime.Now;
            student.DateLastModified = DateTime.Now;
            await context.Students.AddAsync(student, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<StudentResponse>(student);

            return validationService
                        .CreateSuccessResponse(result, ResponseStatus.Created);
        }


        return validationService
                    .CreateErrorResponse<StudentResponse>("Student already exist");
    }

}
