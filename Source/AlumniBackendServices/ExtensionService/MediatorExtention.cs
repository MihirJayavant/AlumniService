using Application.Common.Models;
using Application.Companies;
using Application.Exams;
using Application.Faculties;
using Application.Students;
using Core.Entities;
using FluentValidation;
using Infrastructure.Behaviors;

namespace AlumniBackendServices.ExtensionService;

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
                    .AddValidation<GetCompanyGraphQL, IQueryable<Company>>()
                    .AddValidation<AddFacultyCommand, FacultyResponse>()
                    .AddValidation<DeleteFacultyCommand, FacultyResponse>()
                    .AddValidation<GetAllFacultiesQuery, PaginatedList<FacultyResponse>>()
                    .AddValidation<GetFacultyQuery, FacultyResponse>()
                    .AddValidation<DeleteFacultyCommand, Faculty>()
                    .AddValidation<DeleteFacultyCommand, Faculty>()
                    .AddValidation<DeleteFacultyCommand, Faculty>()
                );

    public static MediatRServiceConfiguration AddValidation<TRequest, TResponse>
    (this MediatRServiceConfiguration configuration) where TRequest : notnull
        => configuration.AddBehavior<IPipelineBehavior<TRequest, OneOf<TResponse, ErrorType>>, ValidationBehavior<TRequest, TResponse>>();
}
