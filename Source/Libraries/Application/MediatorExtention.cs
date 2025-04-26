using Application.Companies;
using Application.Exams;
using Application.Faculties;
using Application.FurtherStudies;
using Application.Students;
using Infrastructure.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class MeditorExtension
{
    public static void AddMediatorServices(this IServiceCollection services)
    => services.AddValidatorsFromAssembly(typeof(StudentResponse).Assembly)
            .AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(StudentResponse).Assembly)
                    .AddValidation<AddExamCommand, ExamResponse>()
                    .AddValidation<GetExamQuery, PaginatedList<ExamResponse>>()
                    .AddValidation<AddCompanyCommand, CompanyResponse>()
                    .AddValidation<GetCompanyQuery, PaginatedList<CompanyResponse>>()
                    .AddValidation<AddFacultyCommand, FacultyResponse>()
                    .AddValidation<DeleteFacultyCommand, FacultyResponse>()
                    .AddValidation<GetAllFacultiesQuery, PaginatedList<FacultyResponse>>()
                    .AddValidation<GetFacultyQuery, FacultyResponse>()
                    .AddValidation<AddFurtherStudyCommand, FurtherStudyResponse>()
                    .AddValidation<GetFurtherStudyQuery, PaginatedList<FurtherStudyResponse>>()
                    .AddValidation<AddStudentCommand, StudentResponse>()
                    .AddValidation<GetAllStudentQuery, AllStudentResponse>()
                    .AddValidation<GetStudentQuery, StudentResponse>()
                );

    public static MediatRServiceConfiguration AddValidation<TRequest, TResponse>
    (this MediatRServiceConfiguration configuration) where TRequest : notnull
        => configuration.AddBehavior<IPipelineBehavior<TRequest, OneOf<TResponse, ErrorType>>, ValidationBehavior<TRequest, TResponse>>();
}
