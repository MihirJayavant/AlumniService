using Application.Common.Models;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Application.Students;

public sealed class GetStudentQuery : IRequest<OneOf<StudentResponse, ErrorType>>
{
    public string Email { get; }
    public GetStudentQuery(string email) => Email = email;
}

public sealed class GetStudentQueryValidator : AbstractValidator<GetStudentQuery>
{
    public GetStudentQueryValidator() => RuleFor(x => x.Email).EmailAddress();
}

public class GetStudentHandler : IRequestHandler<GetStudentQuery, OneOf<StudentResponse, ErrorType>>
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;
    public GetStudentHandler(IApplicationDbContext context, IMapper mapper)
                    => (this.context, this.mapper) = (context, mapper);

    public Task<OneOf<StudentResponse, ErrorType>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {

        return ValidationHelper.ValidateAndRun(request, new GetStudentQueryValidator(), GetData);

        async Task<OneOf<StudentResponse, ErrorType>> GetData()
        {
            var result = await context.Students
                                        .Where(s => s.Email == request.Email)
                                        .ProjectTo<StudentResponse>(mapper.ConfigurationProvider)
                                        .FirstOrDefaultAsync(cancellationToken);
            if (result is null)
            {
                return new ErrorType(ResponseStatus.NotFound, "Student not found");
            }

            return result;
        }
    }

}
