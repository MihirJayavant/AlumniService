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
using Infrastructure.Validators;

namespace Infrastructure.Handlers
{
    public class AddCompanyHandler : BaseHandler, IRequestHandler<AddCompanyCommand, Response<CompanyResponse>>
    {

        public AddCompanyHandler(ApplicationContext context, IMapper mapper, IValidationService validationService)
                        :base(context, mapper, validationService) {}

        public async Task<Response<CompanyResponse>> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
        {

            return await validationService
                        .Compose(request, new AddCompanyValidator(), HandleDatabase);


            async Task<Response<CompanyResponse>> HandleDatabase()
            {
                var student = await context.Students
                                .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

                if(student == null)
                {
                    return validationService.CreateErrorResponse<CompanyResponse>(
                        "Student not found",
                        ResponseStatus.NotFound
                    );
                }

                var company = mapper.Map<Company>(request);
                await context.Companies.AddAsync(company, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                var result = mapper.Map<CompanyResponse>(company);

                return validationService.CreateSuccessResponse(result);

            }

        }

    }
}
