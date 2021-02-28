using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Core.Entities;
using Infrastructure.Commands;
using Database;
using Core.Contracts.Response;
using Infrastructure.Services;

namespace Infrastructure.Handlers
{
    public class AddExamHandler : BaseHandler, IRequestHandler<AddExamCommand, Response<ExamResponse>>
    {

        public AddExamHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        : base(context, mapper, validationService) {}

        public async Task<Response<ExamResponse>> Handle(AddExamCommand request, CancellationToken cancellationToken)
        {
            var student = await context.Students
                            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

            if(student is null)
            {
                return validationService
                            .CreateErrorResponse<ExamResponse>("Student not found", ResponseStatus.NotFound);
            }

            var exam = mapper.Map<Exam>(request);
            await context.Exams.AddAsync(exam, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            var response = mapper.Map<ExamResponse>(exam);

            return validationService.CreateSuccessResponse(response, ResponseStatus.Created);
        }

    }
}
