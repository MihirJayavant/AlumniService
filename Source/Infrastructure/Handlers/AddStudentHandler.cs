using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Core.Entities;
using Infrastructure.Commands;
using Database;
using System;
using Infrastructure.Services;
using Core.Contracts.Response;

namespace Infrastructure.Handlers
{
    public class AddStudentHandler : BaseHandler, IRequestHandler<AddStudentCommand, Response<StudentResponse>>
    {
        public AddStudentHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        : base(context, mapper, validationService) {}

        public async Task<Response<StudentResponse>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
        {
            var account = await context.Students
                            .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

            if(account is null)
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
}
